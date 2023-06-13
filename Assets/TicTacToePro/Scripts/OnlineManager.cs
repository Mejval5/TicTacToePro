using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

namespace TicTacToePro
{
    public class OnlineManager : MonoBehaviour
    {
        public static OnlineManager shared;

        public UnityEvent OnDisconnect;

        void Awake()
        {
            shared = this;
        }

        public IEnumerator CheckInternetConnection(Action<bool> action)
        {

            List<string> servers = new List<string>()
            {
                "https://google.com",
                "https://amazon.com",
                "https://facebook.com/",
                "https://reddit.com/",
                "https://time.is",
                "https://stackoverflow.com/",
            };

            List<UnityWebRequest> requests = new();

            foreach (var server in servers)
            {
                //Debug.Log("checking: " + server);
                var request = UnityWebRequest.Head(server);
                request.timeout = 16;
                request.SendWebRequest();

                requests.Add(request);
            }

            var areRequestsRunning = true;
            while (areRequestsRunning)
            {
                var isAnyoneRunning = false;
                foreach (var request in requests)
                {
                    if (!request.isDone)
                        isAnyoneRunning = true;
                }

                areRequestsRunning = isAnyoneRunning;
                yield return null;
            }

            foreach (var request in requests)
            {
                if (string.IsNullOrEmpty(request.error))
                {
                    //Debug.Log("found: " + request.url);

                    try
                    {
                        string date = request.GetResponseHeader("DATE");
                        action(true);
                        yield break;
                    }
                    catch
                    {
                        continue;
                    }
                }
            }

            action(false);
        }
    }
}