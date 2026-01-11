using Cysharp.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public static class FirebaseInitializer
    {
        public static bool isInitialized { get; private set; }

        public static async UniTask Initialize()
        {
            if (isInitialized)
                return;

            var status = await FirebaseApp.CheckAndFixDependenciesAsync().AsUniTask();
            if (status == DependencyStatus.Available)
            {
                FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                FirebaseAnalytics.LogEvent("firebase_ready");
                isInitialized = true;
                Debug.Log($"[{nameof(FirebaseInitializer)}] Firebase initialized.");
            }
            else
            {
                Debug.LogError($"[{nameof(FirebaseInitializer)}] Firebase initialization failed.");
            }
        }
    }
}