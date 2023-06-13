using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace TicTacToePro
{
    [ExecuteAlways]
    public class ScrollRectGravity : UIBehaviour, IPointerDownHandler, IDragHandler, IEndDragHandler, IPointerUpHandler, IBeginDragHandler
    {
        RectTransform RectTransform;

        protected override void Awake()
        {
            RectTransform = (RectTransform)transform;
        }

        protected override void OnEnable()
        {
            LeftButton.AddListener(LeftButtonClick);
            RightButton.AddListener(RightButtonClick);
        }

        protected override void OnDisable()
        {
            LeftButton.RemoveListener(LeftButtonClick);
            RightButton.RemoveListener(RightButtonClick);
        }

        void LeftButtonClick()
        {
            SelectNext(1);
        }

        void RightButtonClick()
        {
            SelectNext(-1);
        }

        protected override void OnRectTransformDimensionsChange() => Resized();

        public ScrollRect Scroller;
        public HorizontalLayoutGroup Content;
        public float Speed;
        public float MinSize = 0.8f;
        public float SpeedThreshold = 1500f;
        public JuicyButton LeftButton;
        public JuicyButton RightButton;
        public int CurrentlySelectedIndex;

        bool _dragging;
        List<RectTransform> _children;
        RectTransform _currentlySelected;

        public void Resized()
        {
            if (RectTransform == null)
                return;
            var offset = Mathf.RoundToInt(RectTransform.rect.height / 2);
            var rectOffset = new RectOffset(-offset, -offset, 0, 0);
            Content.padding = rectOffset;
        }

        protected override void Start()
        {
            Resized();
            Init();
        }
#if UNITY_EDITOR
        protected override void OnValidate()
        {
            Resized();
            if (_children != null)
                _currentlySelected = _children[CurrentlySelectedIndex];
        }
#endif
        public void Init()
        {
            _children = new();
            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                _children.Add(child.GetComponent<RectTransform>());
            }

            CurrentlySelectedIndex = 0;
            _currentlySelected = _children[CurrentlySelectedIndex];
            LayoutRebuilder.ForceRebuildLayoutImmediate(RectTransform);
            var pos = RectTransform.anchoredPosition;
            pos.x -= _currentlySelected.anchoredPosition.x;
            RectTransform.anchoredPosition = pos;
            Scroller.StopMovement();
            UpdateButtons();
        }

        void Gravity()
        {
            var destination = _currentlySelected.anchoredPosition.x;
            var currentPos = -RectTransform.anchoredPosition.x;
            var direction = (destination - currentPos);
            var distance = Mathf.Abs(direction);
            if (distance < 0.01f)
                return;

            var directionSign = Mathf.Sign(direction);
            var diff = (direction * Time.deltaTime * Speed);
            diff = Mathf.Clamp(Mathf.Abs(diff), 0, distance);
            diff *= directionSign;

            var pos = RectTransform.anchoredPosition;
            pos.x -= diff;
            RectTransform.anchoredPosition = pos;
        }

        void ClosestPosition()
        {
            var currentPos = -RectTransform.anchoredPosition.x;
            RectTransform closestPoint = _currentlySelected;
            var closestDistance = Mathf.Infinity;
            foreach (var item in _children)
            {
                var position = item.anchoredPosition.x;
                var distance = Mathf.Abs(position - currentPos);
                if (distance < closestDistance)
                {
                    closestPoint = item;
                    closestDistance = distance;
                }
            }

            _currentlySelected = closestPoint;
            CurrentlySelectedIndex = _children.IndexOf(_currentlySelected);
            UpdateButtons();
        }

        void LateUpdate()
        {
            Resize();
            if (_dragging)
                return;

            Gravity();
        }

        void Resize()
        {
            var currentPos = -RectTransform.anchoredPosition.x;
            foreach (var item in _children)
            {
                var position = item.anchoredPosition.x;
                var distance = Mathf.Abs(position - currentPos);

                var distanceScaled = distance / RectTransform.rect.height - 0.1f;
                var distanceClamped = Mathf.Clamp(distanceScaled, 0f, 1f);

                var size = Mathf.Lerp(1f, MinSize, distanceClamped);

                item.localScale = new Vector3(size, size, size);
            }
        }

        void SelectNewDestination()
        {
            var velocity = Scroller.velocity.x;
            var speed = Mathf.Abs(velocity);
            if (speed >= SpeedThreshold)
            {
                var dir = Mathf.RoundToInt(Mathf.Sign(velocity));
                SelectNext(dir);
            }
            else
            {
                ClosestPosition();
            }
        }

        public void SelectNext(int dir)
        {
            var index = _children.IndexOf(_currentlySelected) - dir;

            if (index < 0 || index > _children.Count - 1)
                ClosestPosition();
            else
            {
                CurrentlySelectedIndex = index;
                _currentlySelected = _children[CurrentlySelectedIndex];
            }

            UpdateButtons();
        }

        public void UpdateButtons()
        {
            var activeLeft = CurrentlySelectedIndex > 0;
            var activeRight = CurrentlySelectedIndex < _children.Count - 1;
            LeftButton.gameObject.SetActive(activeLeft);
            RightButton.gameObject.SetActive(activeRight);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            _dragging = true;
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerDownHandler);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnDrag(PointerEventData eventData)
        {
            _dragging = true;
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
            SelectNewDestination();
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            _dragging = false;
            ExecuteEvents.ExecuteHierarchy(transform.parent.gameObject, eventData, ExecuteEvents.pointerUpHandler);

        }
    }
}