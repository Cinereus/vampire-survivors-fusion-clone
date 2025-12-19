using System.Collections.Generic;
using CodeBase.Infrastructure.Services.Analytics.Events;
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
        
        public void LogEvent(IAnalyticsEvent analyticsEvent)
        {
            var parameters = analyticsEvent.parameters;
            var firebaseParams = new List<Parameter>(parameters.Count);
            foreach (var parameter in parameters) 
                firebaseParams.Add(new Parameter(parameter.Key, parameter.Value));

            if (!Debug.isDebugBuild)
                FirebaseAnalytics.LogEvent(analyticsEvent.eventName, firebaseParams);

            Debug.Log($"[{nameof(FirebaseAnalyticsService)}] [{analyticsEvent.eventName} params: {string.Join(",", parameters)}].");
        }
    }
}