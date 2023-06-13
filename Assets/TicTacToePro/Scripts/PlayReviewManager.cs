// using Google.Play.Review;
using System.Collections;
using UnityEngine;

namespace TicTacToePro
{
    /// <summary>
    /// Google play review manager
    /// Needs google play API
    /// </summary>
    public class PlayReviewManager : MonoBehaviour
    {
        public static PlayReviewManager shared;

        // ReviewManager _reviewManager;
        // PlayReviewInfo _playReviewInfo;

        void Awake()
        {
            shared = this;
            // _reviewManager = new ReviewManager();
        }

        public IEnumerator ReviewLoop()
        {
            // var requestFlowOperation = _reviewManager.RequestReviewFlow();
            // yield return requestFlowOperation;
            // if (requestFlowOperation.Error != ReviewErrorCode.NoError)
            // {
            //     Debug.Log(requestFlowOperation.Error.ToString());
            //     yield break;
            // }
            // _playReviewInfo = requestFlowOperation.GetResult();
            //
            // var launchFlowOperation = _reviewManager.LaunchReviewFlow(_playReviewInfo);
            // yield return launchFlowOperation;
            // _playReviewInfo = null; // Reset the object
            // if (launchFlowOperation.Error != ReviewErrorCode.NoError)
            // {
            //     Debug.Log(launchFlowOperation.Error.ToString());
            // }
            yield break;
        }
    }
}
