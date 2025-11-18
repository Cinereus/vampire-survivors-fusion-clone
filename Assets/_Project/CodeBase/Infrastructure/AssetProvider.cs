using System;
using System.Collections.Generic;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.UI;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class AssetProvider : IDisposable
    {
        private const string PREFABS_PATH = "prefabs";
        private const string PREFABS_UI_PATH = PREFABS_PATH + "/UI";
        private const string PREFABS_GAME_PATH = PREFABS_PATH + "/Game";
        private const string SPAWNERS_PATH = PREFABS_GAME_PATH + "/Spawners";
        private const string CONFIGS_PATH = "Configs";

        private readonly Dictionary<ItemType, GameObject> _itemPrefabs = new Dictionary<ItemType, GameObject>();
        private readonly Dictionary<HeroType, GameObject> _heroPrefabs = new Dictionary<HeroType, GameObject>();

        private readonly Dictionary<HeroType, GameObject> _heroProjectilePrefabs =
            new Dictionary<HeroType, GameObject>();

        private readonly Dictionary<EnemyType, GameObject> _enemyPrefabs = new Dictionary<EnemyType, GameObject>();
        private readonly Dictionary<string, ScriptableObject> _configs = new Dictionary<string, ScriptableObject>();
        private readonly Dictionary<string, BaseUIEntity> _uiPrefabs = new Dictionary<string, BaseUIEntity>();

        private GameObject _heroSpawnerPrefab;
        private GameObject _enemySpawnerPrefab;
        private GameObject _clientUserHudPrefab;
        private GameObject _remoteUserHudPrefab;
        private GameObject _gameOverScreenPrefab;
        private GameObject _networkRunnerPrefab;
        private GameObject _networkRunnerCallbacksPrefab;

        public void Dispose()
        {
            _itemPrefabs.Clear();
            _heroPrefabs.Clear();
            _heroProjectilePrefabs.Clear();
            _enemyPrefabs.Clear();
            _configs.Clear();
            _uiPrefabs.Clear();
        }

        public TConfig GetConfig<TConfig>(string name = "") where TConfig : ScriptableObject
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(TConfig).Name;

            if (!_configs.TryGetValue(name, out ScriptableObject result))
            {
                result = LoadAtPath<ScriptableObject>($"{CONFIGS_PATH}/{name}");
                _configs[name] = result;
            }

            return (TConfig)result;
        }

        public TEntity GetUIEntity<TEntity>() where TEntity : BaseUIEntity
        {
            var name = typeof(TEntity).Name;
            if (!_uiPrefabs.TryGetValue(name, out BaseUIEntity result))
            {
                result = LoadAtPath<GameObject>($"{PREFABS_UI_PATH}/{name}")?.GetComponent<TEntity>();
                _uiPrefabs[name] = result;
            }

            return (TEntity)result;
        }

        public GameObject GetNetworkRunnerPrefab()
        {
            if (_networkRunnerPrefab == null)
                _networkRunnerPrefab = LoadNetworkRunner();

            return _networkRunnerPrefab;
        }

        public GameObject GetNetworkRunnerCallbacksPrefab()
        {
            if (_networkRunnerCallbacksPrefab == null)
                _networkRunnerCallbacksPrefab = LoadNetworkRunnerCallbacks();

            return _networkRunnerCallbacksPrefab;
        }

        public GameObject GetEnemySpawner()
        {
            if (_enemySpawnerPrefab == null)
                _enemySpawnerPrefab = LoadAtPath<GameObject>($"{SPAWNERS_PATH}/EnemySpawner");

            return _enemySpawnerPrefab;
        }

        public GameObject GetHeroSpawner()
        {
            if (_heroSpawnerPrefab == null)
                _heroSpawnerPrefab = LoadAtPath<GameObject>($"{SPAWNERS_PATH}/HeroSpawner");

            return _heroSpawnerPrefab;
        }

        public GameObject GetEnemy(EnemyType type)
        {
            if (!_enemyPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadAtPath<GameObject>($"{PREFABS_GAME_PATH}/Enemies/{type}");
                _enemyPrefabs[type] = result;
            }

            return result;
        }

        public GameObject GetItem(ItemType type)
        {
            if (!_itemPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadAtPath<GameObject>($"{PREFABS_GAME_PATH}/Items/{type}");
                _itemPrefabs[type] = result;
            }

            return result;
        }

        public GameObject GetHero(HeroType type)
        {
            if (!_heroPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadAtPath<GameObject>($"{PREFABS_GAME_PATH}/Heroes/{type}");
                _heroPrefabs[type] = result;
            }

            return result;
        }

        public GameObject GetProjectile(HeroType type)
        {
            if (!_heroProjectilePrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadHeroProjectile(type);
                _heroProjectilePrefabs[type] = result;
            }

            return result;
        }

        private GameObject LoadNetworkRunner() => LoadAtPath<GameObject>($"{PREFABS_PATH}/NetworkRunner");
        private GameObject LoadNetworkRunnerCallbacks() => LoadAtPath<GameObject>($"{PREFABS_PATH}/NetworkCallbacks");

        private GameObject LoadHeroProjectile(HeroType type) =>
            LoadAtPath<GameObject>($"{PREFABS_GAME_PATH}/Projectiles/{type}Projectile");

        private T LoadAtPath<T>(string path) where T : UnityEngine.Object
        {
            var result = Resources.Load<T>(path);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset at path {path}. Result is null!");

            return result;
        }
    }
}