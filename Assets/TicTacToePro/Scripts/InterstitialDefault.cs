using UnityEngine;
using UnityEngine.Events;

namespace TicTacToePro
{
    public class InterstitialDefault : MonoBehaviour
    {
        public static InterstitialDefault shared;
        public float InterstitialDelay = 60f;

        float _nextInterstitialTime => _lastInterstitialTime + InterstitialDelay;

        float _lastInterstitialTime;

        void Awake()
        {
            if (shared == null)
                shared = this;

            _lastInterstitialTime = Time.realtimeSinceStartup;
        }

        [Header("PlacementID (optional) ")] public string placementID;

        [Space(10)] [Header("Interstitial AD Events (optional) ")] [SerializeField]
        UnityEvent OnInterstitialAdOpened;

        [SerializeField] UnityEvent OnInterstitialAdClosed;
        [SerializeField] UnityEvent OnInterstitialAdError;

        public void OpenInterstitial()
        {
            if (Time.realtimeSinceStartup < _nextInterstitialTime)
                return;

            LogInterstitial("start");
        }

        private void LogInterstitial(string result)
        {
        }


        private void RemoveBindings()
        {
        }

        private void OnInterstitialAdOpenedEvent()
        {
            LogInterstitial("opened");
            OnInterstitialAdOpened.Invoke();
            RemoveBindings();
        }

        private void OnInterstitialAdClosedEvent()
        {
            LogInterstitial("closed");
            OnInterstitialAdClosed.Invoke();
            RemoveBindings();
        }
    }
}