using System;
using System.Collections.Generic;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using Cysharp.Threading.Tasks;
using Firebase.RemoteConfig;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Configs
{
    public class FirebaseConfigProvider : IConfigProvider
    {
        private readonly TextAsset _heroesDefValues;
        private readonly TextAsset _enemiesDefValues;
        private readonly Dictionary<Type, IConfig> _configs = new Dictionary<Type, IConfig>();

        public FirebaseConfigProvider(ConfigDefValues defValues)
        {
            _heroesDefValues = defValues.heroesDefValues;
            _enemiesDefValues = defValues.enemiesDefValues;
        }
        
        public async UniTask Initialize()
        {
            await FirebaseInitializer.Initialize();
            await SetDefaultValues();
            await FetchActualData();
            await ActivateInstance();
            ParseConfigs();
        }

        public T GetConfig<T>() where T : IConfig
        {
            if (_configs.TryGetValue(typeof(T), out IConfig config))
                return (T) config;
            
            Debug.LogError($"Config of type {typeof(T).Name} not found");
            return default;
        }

        private void ParseConfigs()
        {
            ParseHeroesConfig();
            ParseEnemiesConfig();
        }

        private void ParseEnemiesConfig()
        {
            try
            {
                var enemiesJson = FirebaseRemoteConfig.DefaultInstance.GetValue(nameof(EnemiesConfig)).StringValue;
                Debug.Log($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(EnemiesConfig)}\nJson:\n{enemiesJson}");
                _configs[typeof(EnemiesConfig)] = JsonUtility.FromJson<EnemiesConfig>(enemiesJson);
            }
            catch(Exception)
            {
                Debug.LogWarning($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(EnemiesConfig)} failed. Using default values.");
                _configs[typeof(EnemiesConfig)] = JsonUtility.FromJson<EnemiesConfig>(_enemiesDefValues.text);
            }
        }

        private void ParseHeroesConfig()
        {
            try
            {
                var heroesJson = FirebaseRemoteConfig.DefaultInstance.GetValue(nameof(HeroesConfig)).StringValue;
                Debug.Log($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(HeroesConfig)}\nJson:\n{heroesJson}");
                _configs[typeof(HeroesConfig)] = JsonUtility.FromJson<HeroesConfig>(heroesJson);
            }
            catch(Exception)
            {
                Debug.LogWarning($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(HeroesConfig)} failed. Using default values.");
                _configs[typeof(HeroesConfig)] = JsonUtility.FromJson<HeroesConfig>(_enemiesDefValues.text);
            }
        }

        private async UniTask ActivateInstance()
        {
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.ActivateAsync().AsUniTask();
                Debug.Log($"[{nameof(FirebaseConfigProvider)}] Instance activation succeeded.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[{nameof(FirebaseConfigProvider)}] Failed to activate instance: {e}");
            }
        }

        private async UniTask FetchActualData()
        {
            try
            {
                await FirebaseRemoteConfig.DefaultInstance.FetchAsync().AsUniTask();
                Debug.Log($"[{nameof(FirebaseConfigProvider)}] Config data fetched successfully.");
            }
            catch (Exception e)
            {
                Debug.LogWarning($"[{nameof(FirebaseConfigProvider)}] Data fetching failed: {e}");
            }
        }

        private async UniTask SetDefaultValues()
        {
            await FirebaseRemoteConfig.DefaultInstance
                .SetDefaultsAsync(
                    new Dictionary<string, object>
                    {
                        { nameof(HeroesConfig), _heroesDefValues.text },
                        { nameof(EnemiesConfig), _enemiesDefValues.text }
                    })
                .AsUniTask();
        }
    }
}