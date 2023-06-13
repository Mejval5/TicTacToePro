using UnityEngine;
using System.Threading.Tasks;
// using Firebase;
// using Firebase.Firestore;
// using Firebase.Auth;
// using Firebase.Functions;
// using UnityEngine.UI;
// using GooglePlayGames;
// using GooglePlayGames.BasicApi;
// using UnityEngine.SocialPlatforms;
// using Firebase.Analytics;
using System;
using UnityEngine.Events;

namespace TicTacToePro
{
    public class FireBaseClass : MonoBehaviour
    {
        public UnityEvent OnFirebaseConnected;
        public UnityEvent FireAuthCompleted;

        // public FirebaseApp editorApp;
        // public FirebaseFirestore FirestoreDatabase;
        //
        // public FirebaseAuth fireAuth;
        // public FirebaseUser fireUser;

        public bool IsFirebaseConnected = false;

        public static FireBaseClass shared;

        // public FirebaseFunctions functions;


        public string googleAuthCode;

        public bool Logging = true;

        void Awake()
        {
            if (shared == null)
            {
                shared = this;
            }

            IsFirebaseConnected = false;
            OnFirebaseConnected = new UnityEvent();
        }

        // async void Start()
        // {
        //     return;
        //     if (await CheckDependencies())
        //     {
        //         CreateApp();
        //
        //         SilentSignIn();
        //     }
        //     else
        //     {
        //         Debug.LogError("Cannot get dependencies!");
        //     }
        // }

        public void FirebaseConnect()
        {
            OnFirebaseConnected.Invoke();
            IsFirebaseConnected = true;
        }


        void CreateApp()
        {
            // editorApp = FirebaseApp.Create();
            // FirestoreDatabase = FirebaseFirestore.GetInstance(editorApp);
            // functions = FirebaseFunctions.GetInstance(editorApp);
        }

        async Task<bool> CheckDependencies()
        {
            // return await FirebaseApp.CheckAndFixDependenciesAsync() == DependencyStatus.Available;
            await Task.Delay(1);
            return true;
        }

        public void SignInUserWithGooglePlay()
        {
            InitGooglePlay();
        }

        void SilentSignIn()
        {
            SignInUserWithGooglePlay();
            // FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }

        // async void SignInGooglePlayUser(string code)
        // {
        //     googleAuthCode = code;
        //     // await SignInWithGoogleAuthCode();
        //     //if (Logging) Debug.Log("SignInGoogle");
        //     //PlayGamesPlatform.Instance.Authenticate(SignInInteractivity.CanPromptAlways, async (SignInStatus result) =>
        //     //{
        //     //    if (result == SignInStatus.Success)
        //     //    {
        //     //        if (Logging) Debug.Log("Authenthicated");
        //     //        googleAuthCode = PlayGamesPlatform.Instance.GetServerAuthCode();
        //     //        if (Logging) Debug.Log("GotAuth");
        //     //    }
        //     //    else
        //     //    {
        //     //        Debug.Log("Google authenticate failed: " + result.ToString());
        //     //        SceneObjectsManager.shared.SetLoadingScene();
        //     //    }
        //     //});
        // }

        // async Task SignInWithGoogleAuthCode()
        // {
        //     // FirebaseAuth auth = FirebaseAuth.GetAuth(editorApp);
        //     // if (Logging) Debug.Log("AuthFirebase");
        //     // Credential credential = PlayGamesAuthProvider.GetCredential(googleAuthCode);
        //     // if (Logging) Debug.Log("Credential");
        //     // fireUser = await auth.SignInWithCredentialAsync(credential);
        //     // if (Logging) Debug.Log("GotSignInCredential");
        //
        //     FireAuthCompleted.Invoke();
        //     if (Logging) Debug.Log("SignedInGoogle");
        // }

        void InitGooglePlay()
        {
            // PlayGamesPlatform.Instance.Authenticate(ProcessAuthentication);
            // //playGameConfig = new PlayGamesClientConfiguration.Builder().EnableSavedGames().RequestServerAuthCode(false).Build();
            // //PlayGamesPlatform.InitializeInstance(playGameConfig);
            // PlayGamesPlatform.DebugLogEnabled = true;
            // PlayGamesPlatform.Activate();
        }

        // void ProcessAuthentication(SignInStatus status)
        // {
        //     if (status == SignInStatus.Success)
        //     {
        //
        //         PlayGamesPlatform.Instance.RequestServerSideAccess(false, SignInGooglePlayUser);
        //         // Continue with Play Games Services
        //     }
        //     else
        //     {
        //         // Disable your integration with Play Games Services or show a login button
        //         // to ask users to sign-in. Clicking it should call
        //         // PlayGamesPlatform.Instance.ManuallyAuthenticate(ProcessAuthentication).
        //     }
        // }
        //
        // void OnApplicationQuit()
        // {
        //     if (FirestoreDatabase != null)
        //     {
        //         FirestoreDatabase.TerminateAsync();
        //         FirestoreDatabase.ClearPersistenceAsync();
        //     }
        // }

    }
}