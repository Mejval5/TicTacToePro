using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace TicTacToePro
{
    public class JuicyToggle : UIBehaviour, IPointerClickHandler
    {
        public bool Instant;
        public RectTransform Slider;
        public GameObject OnVisual;
        public GameObject OffVisual;
        public UnityEvent<bool> OnClicked;
        public SFXName ClickedSound;

        RectTransform RectTransform;

        bool _enabled;
        float _posOff;
        float _posOn;

        bool _resized;

        protected override void Awake()
        {
            RectTransform = (RectTransform)transform;
        }

        protected override void Start()
        {
            Resized();
        }

        protected override void OnRectTransformDimensionsChange() => _resized = true;

        public void RemoveAllListeners()
        {
            OnClicked.RemoveAllListeners();
        }

        public void AddListener(UnityAction<bool> action)
        {
            OnClicked.AddListener(action);
        }

        void Update()
        {
            if (_resized)
            {
                _resized = false;
                Resized();
            }
        }

        void PlaySound()
        {
            if (SoundManager.shared != null)
            {
                SoundManager.shared.PlaySFX(ClickedSound);
            }
        }

        void Resized()
        {
            if (RectTransform == null)
                RectTransform = (RectTransform)transform;


            ToggleInstant(_enabled);
        }

        public void ToggleAnim(bool toggle)
        {
            var pos = toggle ? _posOn : _posOff;

            StopAllCoroutines();
            StartCoroutine(MoveTowards(0.3f, pos, toggle));
        }

        IEnumerator MoveTowards(float time, float pos, bool toggle)
        {
            var delta = 0f;
            var startPos = Slider.anchoredPosition.x;
            while (delta < time)
            {
                var t = Mathf.SmoothStep(0f, 1f, delta / time);
                var currentPos = Mathf.Lerp(startPos, pos, t);

                Slider.anchoredPosition = new Vector2(currentPos, 0f);

                yield return null;
                delta += Time.deltaTime;
            }

            Slider.anchoredPosition = new Vector2(pos, 0f);
            OnVisual.SetActive(toggle);
            OffVisual.SetActive(!toggle);
        }

        void Clicked()
        {
            _enabled = !_enabled;
            OnClicked.Invoke(_enabled);
            PlaySound();

            if (Instant)
                ToggleInstant(_enabled);
            else
                ToggleAnim(_enabled);
        }

        public void ToggleInstant(bool toggle)
        {
            _enabled = toggle;
            float width = (RectTransform.rect.size.x - Slider.rect.size.x);
            _posOff = -width / 2f;
            _posOn = width / 2f;

            var pos = toggle ? _posOn : _posOff;
            Slider.anchoredPosition = new Vector2(pos, 0f);
            OnVisual.SetActive(toggle);
            OffVisual.SetActive(!toggle);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            Clicked();
        }
    }
}