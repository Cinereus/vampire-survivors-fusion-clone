using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Firebase.Analytics;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public class FirebaseAnalyticsService : IAnalyticsService
    {
        public async UniTask Initialize() => await FirebaseInitializer.Initialize();

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