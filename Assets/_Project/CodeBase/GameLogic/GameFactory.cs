using System;
using System.Collections.Generic;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Attacks;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Components.Hero;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.GameLogic
{
    public class GameFactory : IService
    {
        public event Action<GameObject> onHeroCreated;
        public readonly List<Transform> heroActiveInstances = new List<Transform>();
        
        private readonly HeroesModel _heroes;
        private readonly EnemiesModel _enemies;
        private readonly AssetProvider _assetProvider;
        
        private ServiceLocator serviceLocator => ServiceLocator.instance;

        public GameFactory(HeroesModel heroes, EnemiesModel enemies, AssetProvider assetProvider)
        {
            _heroes = heroes;
            _enemies = enemies;
            _assetProvider = assetProvider;
        }

        public void Dispose() => heroActiveInstances.Clear();

        public GameObject CreateEnemy(EnemyType type, Vector3 spawnPoint)
        {
            var id = Guid.NewGuid();
            EnemyModel model = _enemies.Add(id, type);
            GameObject prefab = _assetProvider.GetEnemy(type);
            GameObject newInstance = Object.Instantiate(prefab, spawnPoint, Quaternion.identity);

            newInstance.GetComponent<Identifier>()?.Setup(id);
            newInstance.GetComponent<HeroChaser>()?.Setup(model.speed, this, heroActiveInstances);
            newInstance.GetComponent<LootSpawner>()?.Setup(model, serviceLocator.Get<LootSpawnService>());
            newInstance.GetComponent<MeleeAttack>()?.Setup(id, model.attackCooldown, serviceLocator.Get<AttackService>());
            newInstance.GetComponent<EnemyDeathHandler>()?.Setup(model);
            return newInstance;
        }

        public GameObject CreateHero(HeroType type, RectTransform hudParent, Vector3 spawnPoint)
        {
            var id = Guid.NewGuid();
            HeroModel model = _heroes.Add(id, type);
            GameObject prefab = _assetProvider.GetHero(type);
            GameObject newInstance = Object.Instantiate(prefab, spawnPoint, Quaternion.identity);
            
            newInstance.GetComponent<Identifier>()?.Setup(id);
            newInstance.GetComponent<PlayerMovement>()?.Setup(serviceLocator.Get<PlayerInputService>(), model);
            newInstance.GetComponent<RemoteAttack>()?.Setup(id, model, this);
            newInstance.GetComponent<HeroDeathHandler>()?.Setup(model);
            heroActiveInstances.Add(newInstance.transform);
            onHeroCreated?.Invoke(newInstance);
            CreateUserHud(model, hudParent);
            return newInstance;
        }
        
        public GameObject CreateUserHud(HeroModel model, RectTransform hudParent)
        {
            GameObject prefab = _assetProvider.GetUserHud();
            GameObject newInstance = Object.Instantiate(prefab, hudParent);
            
            newInstance.GetComponent<UserHud>()?.Setup(model);
            return newInstance;
        }

        public GameObject CreateProjectile(Guid id, Vector3 spawnPoint, Vector3 targetPos)
        {
            if (_heroes.TryGetHeroBy(id, out var hero))
            {
                GameObject prefab = _assetProvider.GetProjectile(hero.type);
                GameObject newInstance = Object.Instantiate(prefab, spawnPoint, Quaternion.identity);
                
                newInstance.GetComponent<AttackProjectile>()?.Setup(id, targetPos, serviceLocator.Get<AttackService>());
                return newInstance;
            }
            return null;
        }
        
        public GameObject CreateItem(ItemType item, Vector3 spawnPoint)
        {
            GameObject prefab = _assetProvider.GetItem(item);
            GameObject newInstance = Object.Instantiate(prefab, spawnPoint, Quaternion.identity);
            
            newInstance.GetComponent<CollectableItem>()?.Setup(item, serviceLocator.Get<ItemsService>());
            return newInstance;
        }
    }
}