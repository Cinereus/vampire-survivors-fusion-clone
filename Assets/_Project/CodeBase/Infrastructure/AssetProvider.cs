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
        private const string USER_HUD_PATH = UI_PREFABS_PATH + "UserHud";
        private const string ENEMY_MODELS_PATH = GAME_PREFABS_PATH + "Enemies/";
        private const string HERO_MODELS_PATH = GAME_PREFABS_PATH + "Heroes/";
        private const string ITEM_MODELS_PATH = GAME_PREFABS_PATH + "Items/";
        private const string PROJECTILE_PATH = GAME_PREFABS_PATH + "Projectiles/";

        private readonly Dictionary<ItemType, GameObject> _itemPrefabs = new Dictionary<ItemType, GameObject>();
        private readonly Dictionary<HeroType, GameObject> _heroPrefabs = new Dictionary<HeroType, GameObject>();
        private readonly Dictionary<HeroType, GameObject> _heroProjectilePrefabs = new Dictionary<HeroType, GameObject>();
        private readonly Dictionary<EnemyType, GameObject> _enemyPrefabs = new Dictionary<EnemyType, GameObject>();
        private GameObject _userHudPrefab;

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
        
        public GameObject GetUserHud()
        {
            if (_userHudPrefab == null)
                _userHudPrefab = LoadUserHud();
            
            return _userHudPrefab;
        }

        private GameObject LoadEnemy(EnemyType type)
        {
            var result = Resources.Load(ENEMY_MODELS_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }

        private GameObject LoadHero(HeroType type)
        {
            var result = Resources.Load(HERO_MODELS_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }

        private GameObject LoadItem(ItemType type)
        {
            var result = Resources.Load(ITEM_MODELS_PATH + type);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }
        
        private GameObject LoadHeroProjectile(HeroType type)
        {
            var result = Resources.Load(PROJECTILE_PATH + type + "Projectile");
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");

            return (GameObject) result;
        }
        
        private GameObject LoadUserHud()
        {
            var result = Resources.Load(USER_HUD_PATH);
            if (!result)
                Debug.LogError($"{nameof(AssetProvider)} Failed to load asset. Result is null!");
            
            return (GameObject) result;
        }
    }
}