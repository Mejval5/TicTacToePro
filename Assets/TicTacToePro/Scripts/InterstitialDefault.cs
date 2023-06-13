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
            // Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
            // Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent += OnInterstitialAdClosedEvent;
            // Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent += OnInterstitialAdErorEvent;

            // if (Yodo1U3dMas.IsInterstitialAdLoaded())
            // {
            //     if (string.IsNullOrEmpty(placementID))
            //     {
            //         Yodo1U3dMas.ShowInterstitialAd();
            //     }
            //     else
            //     {
            //         Yodo1U3dMas.ShowInterstitialAd(placementID);
            //     }
            //     _lastInterstitialTime = Time.realtimeSinceStartup;
            // }
            // else
            // {
            //     Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad has not been cached.");
            // }
        }

        private void LogInterstitial(string result)
        {
            //FirebaseAnalytics.LogEvent(FirebaseAnalytics.ParameterAdFormat, "interstitial", result);
        }


        private void RemoveBindings()
        {
            // Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent -= OnInterstitialAdOpenedEvent;
            // Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent -= OnInterstitialAdClosedEvent;
            // Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent -= OnInterstitialAdErorEvent;
        }

        private void OnInterstitialAdOpenedEvent()
        {
            LogInterstitial("opened");
            //Debug.Log(Yodo1U3dMas.TAG + "My Interstitial ad opened");
            OnInterstitialAdOpened.Invoke();
            RemoveBindings();
            // Yodo1U3dMasCallback.Instance.UnPause();
        }

        private void OnInterstitialAdClosedEvent()
        {
            LogInterstitial("closed");
            // Debug.Log(Yodo1U3dMas.TAG + "My Interstitial ad closed - breaks");
            OnInterstitialAdClosed.Invoke();
            RemoveBindings();
            // Yodo1U3dMasCallback.Instance.UnPause();
        }

        // private void OnInterstitialAdErorEvent(Yodo1U3dAdError adError)
        // {
        //     LogInterstitial("error");
        //     // Debug.Log(Yodo1U3dMas.TAG + "My Interstitial ad error - " + adError.ToString());
        //     OnInterstitialAdError.Invoke();
        //     RemoveBindings();
        //     // Yodo1U3dMasCallback.Instance.UnPause();
        // }
    }
}