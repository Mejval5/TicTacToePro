using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
// using Yodo1.MAS;

namespace TicTacToePro
{
    public class RewardedDefault : MonoBehaviour
    {
        public static RewardedDefault shared;

        [Header("PlacementID (optional) ")] [Tooltip("Enter your Rewarded Ad placement ID. Leave empty if you do not have one.")]
        public string placementID;

        [Space(10)] [Header("Rewarded AD Events")]
        public UnityEvent OnRewardedAdOpened;

        public UnityEvent OnRewardedAdClosed;
        [Header("Award User Here")] public UnityEvent OnAdReceivedReward;
        public UnityEvent OnRewardedAdError;

        void Awake()
        {
            shared = this;
        }

        public void ShowRewarded()
        {
            if (Application.isEditor)
            {
                OnAdReceivedRewardEvent();
                return;
            }

            OnAdReceivedRewardEvent();
            // if (Yodo1U3dMas.IsRewardedAdLoaded())
            // {
            //     Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;
            //     Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;
            //     Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
            //     Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;
            //     if (string.IsNullOrEmpty(placementID))
            //     {
            //         Yodo1U3dMas.ShowRewardedAd();
            //     }
            //     else
            //     {
            //         Yodo1U3dMas.ShowRewardedAd(placementID);
            //     }
            // }
            // else
            // {
            OnAdReceivedReward.RemoveAllListeners();
            //     Debug.Log(Yodo1U3dMas.TAG + "NoCode Reward video ad has not been cached.");
            // }
        }

        private void OnRewardedAdOpenedEvent()
        {
            OnRewardedAdOpened.Invoke();
            OnRewardedAdOpened.RemoveAllListeners();
        }

        private void OnRewardedAdClosedEvent()
        {
            // Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad closed");
            OnRewardedAdClosed.Invoke();
            // Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent -= OnRewardedAdOpenedEvent;
            // Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent -= OnRewardedAdClosedEvent;
            // Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent -= OnAdReceivedRewardEvent;
            // Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent -= OnRewardedAdErorEvent;

            OnRewardedAdClosed.RemoveAllListeners();
            OnAdReceivedReward.RemoveAllListeners();
        }

        private void OnAdReceivedRewardEvent()
        {
            // Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad received reward");
            OnAdReceivedReward.Invoke();
            OnAdReceivedReward.RemoveAllListeners();
        }

        // private void OnRewardedAdErorEvent(Yodo1U3dAdError adError)
        private void OnRewardedAdErorEvent()
        {
            // Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad error - " + adError.ToString());
            OnRewardedAdError.Invoke();
            OnRewardedAdError.RemoveAllListeners();
        }
    }
}