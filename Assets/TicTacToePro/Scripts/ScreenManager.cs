using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace TicTacToePro
{
    [ExecuteAlways]
    public class ScreenManager : MonoBehaviour
    {
        public static ScreenManager shared;
        public ScreenType CurrentScreenType;
        public GameScreen IntroScreen;
        public GameScreen TTTSelectGameModeScreen;
        public GameScreen TTTGameScreen;
        public GameScreen Main2048Screen;
        public GameScreen GameSelectorScreen;

        public Dictionary<ScreenType, GameScreen> GetAllScreens()
        {
            return new Dictionary<ScreenType, GameScreen>()
            {
                { ScreenType.Intro, IntroScreen },
                { ScreenType.TTTSelectGameMode, TTTSelectGameModeScreen },
                { ScreenType.TTTGame, TTTGameScreen },
                { ScreenType.Main2048, Main2048Screen },
                { ScreenType.GameSelector, GameSelectorScreen },
            };
        }

        public GameScreen GetScreenByType(ScreenType screenType)
        {
            switch (screenType)
            {
                case ScreenType.Intro:
                    return IntroScreen;
                case ScreenType.TTTSelectGameMode:
                    return TTTSelectGameModeScreen;
                case ScreenType.TTTGame:
                    return TTTGameScreen;
                case ScreenType.Main2048:
                    return Main2048Screen;
                case ScreenType.GameSelector:
                    return GameSelectorScreen;
                default:
                    return null;
            }
        }

        public List<GameObject> HideObjectsIn2048;

        public ScreenType LastScreenType;

        public Animator CurrentScreenAnimator
        {
            get { return GetScreenByType(CurrentScreenType).GetAnimator; }
        }

        public GameScreen CurrentScreen
        {
            get { return GetScreenByType(CurrentScreenType); }
        }

        void Awake()
        {
            if (!Application.isEditor)
            {
                CurrentScreenType = ScreenType.Intro;
                LastScreenType = ScreenType.Intro;
            }

            if (shared == null)
                shared = this;
        }

        void Start()
        {
            if (shared == null)
                shared = this;

            if (!Application.isEditor)
                CurrentScreenType = ScreenType.Intro;

            DisableScreens();
            LastScreenType = CurrentScreenType;
            CurrentScreen.Show();

            if (SoundManager.shared != null)
                SoundManager.shared.PlayMusic(CurrentScreenType);
        }

        void OnValidate()
        {
            if (shared == null)
                shared = this;

            if (!Application.isPlaying)
            {
                DisableScreens();
                CurrentScreen.Show();
                UpdateEnabledObjects();
            }
        }

        public void DisableScreens()
        {
            foreach (var screenType in Enum.GetValues(typeof(ScreenType)))
            {
                if ((ScreenType)screenType != CurrentScreenType)
                    GetScreenByType((ScreenType)screenType).Idle();
            }
        }

        public void SelectLastScreen()
        {
            SelectScreen(LastScreenType);
        }

        public void SelectScreen(ScreenType screenType)
        {
            if (GetScreenByType(screenType) == null)
                return;

            if (CurrentScreenType == screenType)
                return;

            LastScreenType = CurrentScreenType;
            CurrentScreen.Hide();

            CurrentScreenType = screenType;
            CurrentScreen.Show();

            UpdateEnabledObjects();
        }

        void UpdateEnabledObjects()
        {
            foreach (var enabledObject in HideObjectsIn2048)
            {
                var isEnabled = CurrentScreenType != ScreenType.Main2048;
                var go = enabledObject;
                var image = go.GetComponent<Graphic>();
                if (image != null && Application.isPlaying)
                {
                    if (isEnabled == false)
                        StartCoroutine(AlphaAnimation(image, 0, 0.4f, true));
                    else
                    {
                        go.SetActive(isEnabled);
                        StartCoroutine(AlphaAnimation(image, 1, 0.4f));
                    }
                }
                else
                {
                    go.SetActive(isEnabled);
                }

            }
        }

        IEnumerator AlphaAnimation(Graphic graphic, float targetAlpha, float time, bool disableAtEnd = false)
        {
            // do the animation with inoutquad, loop back to the original scale
            float t = 0f;
            float startAlpha = graphic.color.a;
            while (t < time)
            {
                t += Time.unscaledDeltaTime;
                float lerpT = Mathf.Sin(t / (time) * Mathf.PI * 0.5f);
                Color color = graphic.color;
                color.a = Mathf.Lerp(startAlpha, targetAlpha, lerpT);
                graphic.color = color;
                yield return null;
            }

            Color finalColor = graphic.color;
            finalColor.a = targetAlpha;
            graphic.color = finalColor;
            if (disableAtEnd)
            {
                graphic.gameObject.SetActive(false);
            }

            yield return null;
        }
    }

    [Serializable]
    public enum ScreenType
    {
        Intro,
        TTTSelectGameMode,
        TTTGame,
        Main2048,
        GameSelector,
    }

    [Serializable]
    public enum ScreenTransition
    {
        Show,
        Hide,
        Idle,
    }
}