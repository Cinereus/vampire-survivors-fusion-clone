using System.Collections.Generic;
using Firebase.Analytics;
using Firebase.Extensions;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public class FirebaseAnalyticsService : IAnalyticsService
    {
        public void Initialize()
        {
            Firebase.FirebaseApp
                .CheckAndFixDependenciesAsync()
                .ContinueWithOnMainThread(task => 
                {
                    if (task.Result == Firebase.DependencyStatus.Available)
                    {
                        FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);
                        FirebaseAnalytics.LogEvent("firebase_ready");
                        Debug.Log($"[{nameof(FirebaseAnalyticsService)}] Initialized.");
                    }
                });
        }
        
        public void LogEvent(string eventName, params (string name, string val)[] parameters)
        {
            var firebaseParams = new List<Parameter>(parameters.Length);
            foreach (var parameter in parameters) 
                firebaseParams.Add(new Parameter(parameter.name, parameter.val));

            if (!Debug.isDebugBuild)
                FirebaseAnalytics.LogEvent(eventName, firebaseParams);
            
            Debug.Log($"[{nameof(FirebaseAnalyticsService)}] [{eventName} params: {string.Join(",", parameters)}].");
        }
    }
}