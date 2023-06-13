using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace TicTacToePro
{
    public class GameScreen : MonoBehaviour
    {
        public List<GameScreen> Children;
        public float TransitionTime = 0.75f;

        [HideInInspector] public UnityEvent OnShow;

        [HideInInspector] public UnityEvent OnHide;


        Canvas _canvas;
        protected Animator _animator;

        public virtual void Awake()
        {
            EnsureAllComponents();
        }

        public virtual void OnValidate()
        {
            EnsureAllComponents();
        }

        public virtual void EnsureAllComponents()
        {
            this.EnsureComponent(ref _canvas);
            this.EnsureComponent<CanvasGroup>();
            this.EnsureComponent<GraphicRaycaster>();
            this.EnsureComponent(ref _animator);
        }

        public Animator GetAnimator
        {
            get
            {
                if (_animator == null)
                    _animator = GetComponent<Animator>();
                return _animator;
            }
        }

        public Canvas GetCanvas
        {
            get
            {
                if (_canvas == null)
                    _canvas = GetComponent<Canvas>();
                return _canvas;
            }
        }

        public virtual void Show()
        {
            ShowSelf();

            foreach (var childScreen in Children)
            {
                childScreen.ShowSelf();
            }
        }

        public virtual void ShowSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Show.ToString());
            if (!Application.isPlaying)
                GetCanvas.enabled = true;
            else
                StartCoroutine(DelayAction(OnShow, TransitionTime));
        }

        public virtual void Hide()
        {
            HideSelf();

            foreach (var childScreen in Children)
            {
                childScreen.HideSelf();
            }
        }

        public virtual void HideSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Hide.ToString());
            if (!Application.isPlaying)
                GetCanvas.enabled = false;
            else
                StartCoroutine(DelayAction(OnHide, TransitionTime));
        }

        public virtual void Idle()
        {
            IdleSelf();

            foreach (var childScreen in Children)
            {
                childScreen.IdleSelf();
            }
        }

        public virtual void IdleSelf()
        {
            GetAnimator.SetTrigger(ScreenTransition.Idle.ToString());
            if (!Application.isPlaying)
                GetCanvas.enabled = false;
        }

        public IEnumerator DelayAction(UnityEvent action, float delay)
        {
            yield return new WaitForSeconds(delay);
            action.Invoke();
        }

        [Button(nameof(SelectScreen))] public bool selectScreen;

        public void SelectScreen()
        {
            if (Application.isPlaying == false)
            {
                foreach (GameScreen screen in FindObjectsOfType<GameScreen>())
                {
                    screen.IdleSelf();
                }
                
                Show();
            }
            
            foreach (var item in ScreenManager.shared.GetAllScreens())
            {
                if (item.Value == this)
                {
                    ScreenManager.shared.SelectScreen(item.Key);
                    return;
                }
            }

            Show();
        }
    }
}