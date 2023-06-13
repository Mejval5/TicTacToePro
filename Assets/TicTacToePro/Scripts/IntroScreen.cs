using System.Collections;
using UnityEngine;

namespace TicTacToePro
{
    public class IntroScreen : MonoBehaviour
    {
        public float WaitTime = 1f;
        public float WaitTimeLong = 2f;
        public bool EditorSkip = true;

        void Awake()
        {
            StartCoroutine(StartGameCoroutine(WaitTimeLong));
        }

        public void StartGame()
        {
            StopAllCoroutines();
            StartCoroutine(StartGameCoroutine(WaitTime));
        }

        IEnumerator StartGameCoroutine(float time)
        {
            yield return new WaitForSeconds(time);

            if (!Application.isEditor || !EditorSkip)
                ScreenManager.shared.SelectScreen(ScreenType.GameSelector);
        }
    }
}