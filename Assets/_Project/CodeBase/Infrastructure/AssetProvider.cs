using System.Collections.Generic;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.Infrastructure
{
    public class AssetProvider : IService
    {
        private const string GAME_PREFABS_PATH = "Prefabs/Game/";
        private const string UI_PREFABS_PATH = "Prefabs/UI/";
        private const string USER_HUD_PATH = UI_PREFABS_PATH + "ClientUserHud";
        private const string REMOTE_USER_HUD_PATH = UI_PREFABS_PATH + "RemoteUserHud";
        private const string GAME_OVER_SCREEN_PATH = UI_PREFABS_PATH + "GameOverScreen";
        private const string SPAWNERS_PATH = GAME_PREFABS_PATH + "Spawners/";
        private const string ENEMIES_PATH = GAME_PREFABS_PATH + "Enemies/";
        private const string HEROES_PATH = GAME_PREFABS_PATH + "Heroes/";
        private const string ITEMS_PATH = GAME_PREFABS_PATH + "Items/";
        private const string PROJECTILES_PATH = GAME_PREFABS_PATH + "Projectiles/";

        private readonly Dictionary<ItemType, GameObject> _itemPrefabs = new Dictionary<ItemType, GameObject>();
        private readonly Dictionary<HeroType, GameObject> _heroPrefabs = new Dictionary<HeroType, GameObject>();
        private readonly Dictionary<HeroType, GameObject> _heroProjectilePrefabs = new Dictionary<HeroType, GameObject>();
        private readonly Dictionary<EnemyType, GameObject> _enemyPrefabs = new Dictionary<EnemyType, GameObject>();
        private GameObject _clientUserHudPrefab;
        private GameObject _remoteUserHudPrefab;
        private GameObject _gameOverScreenPrefab;

        public void Dispose()
        {
        }

        public GameObject GetEnemy(EnemyType type)
        {
            if (!_enemyPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadEnemy(type);
                _enemyPrefabs[type] = result;
            }
            return result;
        }

        public GameObject GetItem(ItemType type)
        {
            if (!_itemPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadItem(type);
                _itemPrefabs[type] = result;
            }
            return result;
        }

        public GameObject GetHero(HeroType type)
        {
            if (!_heroPrefabs.TryGetValue(type, out GameObject result))
            {
                result = LoadHero(type);
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
        
        public GameObject GetClientUserHud()
        {
            if (_clientUserHudPrefab == null)
                _clientUserHudPrefab = LoadClientUserHud();
            
            return _clientUserHudPrefab;
        }
        
        public GameObject GetRemoteUserHud()
        {
            if (_remoteUserHudPrefab == null)
                _remoteUserHudPrefab = LoadRemoteUserHud();
            
            return _remoteUserHudPrefab;
        }
        
        public GameObject GetGameOverScreen()
        {
            if (_gameOverScreenPrefab == null)
                _gameOverScreenPrefab = LoadGameOverScreen();
            
            return _gameOverScreenPrefab;
        }
        
        public GameObject GetHeroSpawner()
        {
            var result = Resources.Load(SPAWNERS_PATH + "HeroSpawner");
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }

        public GameObject GetEnemySpawner()
        {
            var result = Resources.Load(SPAWNERS_PATH + "EnemySpawner");
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }

        private GameObject LoadEnemy(EnemyType type)
        {
            var result = Resources.Load(ENEMIES_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }

        private GameObject LoadHero(HeroType type)
        {
            var result = Resources.Load(HEROES_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }

        private GameObject LoadItem(ItemType type)
        {
            var result = Resources.Load(ITEMS_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }
        
        private GameObject LoadHeroProjectile(HeroType type)
        {
            var result = Resources.Load(PROJECTILES_PATH + type + "Projectile");
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }
        
        private GameObject LoadClientUserHud()
        {
            var result = Resources.Load(USER_HUD_PATH);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }   
        
        private GameObject LoadRemoteUserHud()
        {
            var result = Resources.Load(REMOTE_USER_HUD_PATH);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }
        
        private GameObject LoadGameOverScreen()
        {
            var result = Resources.Load(GAME_OVER_SCREEN_PATH);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }
    }
}