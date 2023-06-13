using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TicTacToePro
{
    /// <summary>
    /// This is my custom button script. It creates nice and juicy animation when the player clicks.
    /// <para>If the user moves cursor outside of the button before clicking up then the button cancels.</para>
    /// </summary>
    [RequireComponent(typeof(CanvasGroup))]
    public class JuicyButton : MonoBehaviour
    {
        public bool Interactable = true;
        [Header("Parameters")] public float minPercent = 0.8f;
        public float speed = 9f;
        public float cancelDistance = 40f;
        public float sinScale = 1.2f;
        public float disableAlpha = 0.5f;
        [Header("Event OnClicked")] public UnityEvent clicked;

        private EventTrigger _eventTrigger;
        private Vector3 _originalScale;
        private RectTransform _rect;
        private bool _pressed = false;

        private bool _growAnimationRun;
        private bool _shrinkAnimationRun;

        private float _startPercent;
        private float _currentPercent;
        private float _timeVar;

        private float _startT;
        private float _currentT;
        private float _currentScale;
        private float _stopT;
        private Canvas _mainCanvas;
        private CanvasGroup _canvasGroup;
        public SFXName ClickedSound;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            _eventTrigger = gameObject.AddComponent<EventTrigger>();
            _rect = GetComponent<RectTransform>();
            if (clicked == null)
            {
                clicked = new UnityEvent();
            }

            _originalScale = _rect.localScale;
            _mainCanvas = GetComponentInParent<Canvas>().rootCanvas;
        }


        public void RemoveAllListeners()
        {
            clicked.RemoveAllListeners();
        }

        /// <summary>
        /// Register listener to the clicked event
        /// </summary>
        public void AddListener(UnityAction action)
        {
            clicked.AddListener(action);
        }

        /// <summary>
        /// Unregister listener from the clicked event
        /// </summary>
        public void RemoveListener(UnityAction action)
        {
            clicked.RemoveListener(action);
        }

        private void OnEnable()
        {
            _pressed = false;
            SetupTriggers();
        }

        /// <summary>
        /// This method sets up all triggers for user input
        /// </summary>
        private void SetupTriggers()
        {
            EventTrigger.Entry dragger = new EventTrigger.Entry();
            dragger.eventID = EventTriggerType.Drag;
            dragger.callback.AddListener((data) => { CheckDistanceOfMouse((PointerEventData)data); });
            _eventTrigger.triggers.Add(dragger);

            EventTrigger.Entry pointerDown = new EventTrigger.Entry();
            pointerDown.eventID = EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => { PointerDown((PointerEventData)data); });
            _eventTrigger.triggers.Add(pointerDown);

            EventTrigger.Entry pointerUp = new EventTrigger.Entry();
            pointerUp.eventID = EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => { PointerUp((PointerEventData)data); });
            _eventTrigger.triggers.Add(pointerUp);
        }

        /// <summary>
        /// This method removes all triggers for user input
        /// </summary>
        private void RemoveTriggers()
        {
            _eventTrigger.triggers.Clear();
        }

        /// <summary>
        /// This is a check if the user moved cursor outside of the button while holding down button.
        /// </summary>
        private void CheckDistanceOfMouse(PointerEventData data)
        {
            if (!_pressed)
            {
                return;
            }

            Vector3 pos = RectTransformUtility.WorldToScreenPoint(_mainCanvas.worldCamera, _rect.position);

            float diffX = data.position.x - pos.x;
            float diffY = data.position.y - pos.y;

            Vector2 distanceVector = new Vector2(Mathf.Abs(diffX), Mathf.Abs(diffY));

            bool cancelX = distanceVector.x > (_rect.rect.size.x / 2) + cancelDistance;
            bool cancelY = distanceVector.y > (_rect.rect.size.y / 2) + cancelDistance;

            if (cancelX || cancelY)
            {
                _pressed = false;
                GrowStart();
            }
        }

        /// <summary>
        /// Gets called when user clicks down on the button
        /// </summary>
        private void PointerDown(PointerEventData _)
        {
            if (!Interactable)
                return;

            _pressed = true;
            StopAllAnimations();
            ShrinkStart();
        }

        /// <summary>
        /// Gets called when user clicks up after clicking down
        /// </summary>
        private void PointerUp(PointerEventData _)
        {
            if (!_pressed)
            {
                return;
            }

            StopAllAnimations();
            GrowStart();

            if (Interactable)
                clicked.Invoke();
            PlaySound();
        }

        void PlaySound()
        {
            if (SoundManager.shared != null)
            {
                SoundManager.shared.PlaySFX(ClickedSound);
            }
        }

        /// <summary>
        /// Clears all animations on the button
        /// </summary>
        private void StopAllAnimations()
        {
            _growAnimationRun = false;
            _shrinkAnimationRun = false;
        }

        private void Update()
        {
            if (_shrinkAnimationRun)
            {
                Shrink();
            }

            if (_growAnimationRun)
            {
                Grow();
            }

            ToggleInteractability();
        }


        void ToggleInteractability()
        {
            if (Interactable)
            {
                _canvasGroup.alpha = 1f;
            }
            else
            {
                _canvasGroup.alpha = disableAlpha;
            }
        }

        /// <summary>
        /// Starts shrinking animation
        /// </summary>
        private void ShrinkStart()
        {
            _shrinkAnimationRun = true;
            _startPercent = _rect.localScale.x / _originalScale.x;
            _currentPercent = _startPercent;
            _timeVar = GetTime();
        }

        /// <summary>
        /// Starts growing animation
        /// </summary>
        private void GrowStart()
        {
            _growAnimationRun = true;
            _startPercent = _rect.localScale.x / _originalScale.x;
            _startT = GetAsin(_startPercent);
            _currentT = _startT;
            _timeVar = GetTime();
            _stopT = Mathf.PI - GetAsin(1);
        }

        /// <summary>
        /// Shrinking animation loop
        /// </summary>
        private void Shrink()
        {
            _currentPercent = Mathf.Lerp(_startPercent, minPercent, (GetTime() - _timeVar) * speed);
            _rect.localScale = new Vector3(_currentPercent * _originalScale.x, _currentPercent * _originalScale.y, 1);
            if (_currentPercent <= minPercent)
            {
                _rect.localScale = new Vector3(minPercent * _originalScale.x, minPercent * _originalScale.y, 1);
                _shrinkAnimationRun = false;
            }
        }

        /// <summary>
        /// Growing animation loop
        /// </summary>
        private void Grow()
        {
            _currentT = Mathf.Lerp(_startT, _stopT, (GetTime() - _timeVar) / 2 * speed);
            _currentScale = SinFunc(_currentT);
            _rect.localScale = new Vector3(_currentScale * _originalScale.x, _currentScale * _originalScale.y, 1);
            if (_currentT >= _stopT)
            {
                _rect.localScale = new Vector3(_originalScale.x, _originalScale.y, 1);
                _growAnimationRun = false;
            }
        }

        private void OnDisable()
        {
            RemoveTriggers();
            _rect.localScale = new Vector3(_originalScale.x, _originalScale.y, 1);
            StopAllAnimations();
        }

        /// <summary>
        /// Method to get scaled time easily
        /// </summary>
        private float GetTime()
        {
            return Time.time;
        }

        /// <summary>
        /// Custom Sin function for animations
        /// </summary>
        private float SinFunc(float t)
        {
            return sinScale * Mathf.Sin(t);
        }

        /// <summary>
        /// Custom ASin function for animations
        /// </summary>
        private float GetAsin(float t)
        {
            return Mathf.Asin(t / sinScale);
        }

    }
}