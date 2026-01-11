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
        private const string HEROES_CONFIG_DEF_VALUES = @"{
              ""heroes"": [
                {
                  ""heroType"": 0,
                  ""maxHealth"": 999,
                  ""currentHealth"": 100,
                  ""speed"": 0.5,
                  ""damage"": 1,
                  ""attackCooldown"": 1,
                  ""maxXp"": 20,
                  ""currentXp"": 0,
                  ""currentLevel"": 0,
                  ""progressionCoeff"": 1.5,
                  ""statIncreaseCoeff"": 1.5
                },
                {
                  ""heroType"": 1,
                  ""maxHealth"": 120,
                  ""currentHealth"": 120,
                  ""speed"": 0.2,
                  ""damage"": 3,
                  ""attackCooldown"": 2.5,
                  ""maxXp"": 30,
                  ""currentXp"": 0,
                  ""currentLevel"": 0,
                  ""progressionCoeff"": 1.5,
                  ""statIncreaseCoeff"": 1.2
                },
                {
                  ""heroType"": 2,
                  ""maxHealth"": 70,
                  ""currentHealth"": 70,
                  ""speed"": 0.3,
                  ""damage"": 4,
                  ""attackCooldown"": 4,
                  ""maxXp"": 15,
                  ""currentXp"": 0,
                  ""currentLevel"": 0,
                  ""progressionCoeff"": 1.5,
                  ""statIncreaseCoeff"": 2
                }
              ]
            }";
        
        private const string ENEMIES_CONFIG_DEF_VALUES = @"{
              ""enemies"": [
                {
                  ""enemyType"": 2,
                  ""maxHealth"": 2,
                  ""currentHealth"": 2,
                  ""damage"": 2,
                  ""attackCooldown"": 1.5,
                  ""speed"": 0.2,
                  ""lootProbability"": 15,
                  ""spawnProbability"": 50
                },
                {
                  ""enemyType"": 22,
                  ""maxHealth"": 8,
                  ""currentHealth"": 8,
                  ""damage"": 15,
                  ""attackCooldown"": 2,
                  ""speed"": 0.08,
                  ""lootProbability"": 80,
                  ""spawnProbability"": 20
                },
                {
                  ""enemyType"": 67,
                  ""maxHealth"": 3,
                  ""currentHealth"": 3,
                  ""damage"": 2,
                  ""attackCooldown"": 1.5,
                  ""speed"": 0.2,
                  ""lootProbability"": 15,
                  ""spawnProbability"": 50
                },
                {
                  ""enemyType"": 37,
                  ""maxHealth"": 1,
                  ""currentHealth"": 1,
                  ""damage"": 1,
                  ""attackCooldown"": 1.5,
                  ""speed"": 0.3,
                  ""lootProbability"": 15,
                  ""spawnProbability"": 50
                },
                {
                  ""enemyType"": 8,
                  ""maxHealth"": 15,
                  ""currentHealth"": 15,
                  ""damage"": 25,
                  ""attackCooldown"": 2.5,
                  ""speed"": 0.05,
                  ""lootProbability"": 50,
                  ""spawnProbability"": 15
                }
              ]
            }";
        
        private readonly Dictionary<Type, IConfig> _configs = new Dictionary<Type, IConfig>();

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
            var heroesJson = FirebaseRemoteConfig.DefaultInstance.GetValue(nameof(HeroesConfig)).StringValue;
            Debug.Log($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(HeroesConfig)}\nJson:\n{heroesJson}");
            _configs[typeof(HeroesConfig)] = JsonUtility.FromJson<HeroesConfig>(heroesJson);
            
            var enemiesJson = FirebaseRemoteConfig.DefaultInstance.GetValue(nameof(EnemiesConfig)).StringValue;
            Debug.Log($"[{nameof(FirebaseConfigProvider)}]: Parsing {nameof(EnemiesConfig)}\nJson:\n{enemiesJson}");
            _configs[typeof(EnemiesConfig)] = JsonUtility.FromJson<EnemiesConfig>(enemiesJson);
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
                        { nameof(HeroesConfig), HEROES_CONFIG_DEF_VALUES },
                        { nameof(EnemiesConfig), ENEMIES_CONFIG_DEF_VALUES }
                    })
                .AsUniTask();
        }
    }
}