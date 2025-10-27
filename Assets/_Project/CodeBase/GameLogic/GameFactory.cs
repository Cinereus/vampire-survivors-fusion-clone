using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.EntryPoints;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Attacks;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Components.Hero;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using CodeBase.UI;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic
{
    public class GameFactory : IService
    {
        private readonly HeroesModel _heroes;
        private readonly EnemiesModel _enemies;
        private readonly NetworkContainer _network;
        private readonly HeroesInstanceProvider _instanceProvider;
        private readonly AssetProvider _assetProvider;
        private readonly RectTransform _uiPlaceholder;

        private ServiceLocator serviceLocator => ServiceLocator.instance;

        public GameFactory(HeroesModel heroes, EnemiesModel enemies, AssetProvider assetProvider,
            HeroesInstanceProvider instanceProvider, RectTransform uiPlaceholder, NetworkContainer network)
        {
            _heroes = heroes;
            _enemies = enemies;
            _network = network;
            _instanceProvider = instanceProvider;
            _assetProvider = assetProvider;
            _uiPlaceholder = uiPlaceholder;
        }

        public GameObject CreateEnemy(EnemyType type, Vector3 spawnPoint)
        {
            GameObject prefab = _assetProvider.GetEnemy(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity);
            var id = newInstance.Id.Raw;
            EnemyModel model = _enemies.Add(id, type);
            
            newInstance.GetComponent<Identifier>()?.Setup(id);
            newInstance.GetComponent<HeroChaser>()?.Setup(model.speed, _instanceProvider);
            newInstance.GetComponent<LootSpawner>()?.Setup(model, serviceLocator.Get<LootSpawnService>());
            newInstance.GetComponent<MeleeAttack>()?.Setup(id, model.attackCooldown, serviceLocator.Get<AttackService>());
            newInstance.GetComponent<EnemyDeathHandler>()?.Setup(model);
            return newInstance.gameObject;
        }

        public NetworkObject CreateHero(HeroType type, Vector2 spawnPoint)
        {
            GameObject prefab = _assetProvider.GetHero(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity);
            var id = newInstance.Id.Raw;
            HeroModel model = _heroes.Add(id, type);
            
            newInstance.GetComponent<Identifier>()?.Setup(id);
            newInstance.GetComponent<PlayerMovement>()?.Setup(serviceLocator.Get<PlayerInputService>(), model);
            newInstance.GetComponent<RemoteAttack>()?.Setup(id, model, this);
            newInstance.GetComponent<HeroDeathHandler>()?.Setup(model, this);
            
            _instanceProvider.AddHero(id, newInstance);
            CreateUserHud(model);
            return newInstance;
        }
        
        public GameObject CreateUserHud(HeroModel model)
        {
            GameObject prefab = _assetProvider.GetUserHud();
            NetworkObject newInstance = _network.runner.Spawn(prefab);
            
            newInstance.transform.SetParent(_uiPlaceholder);
            newInstance.GetComponent<UserHud>()?.Setup(model);
            return newInstance.gameObject;
        }
        
        public GameObject CreateGameOverScreen()
        {
            GameObject prefab = _assetProvider.GetGameOverScreen();
            NetworkObject newInstance = _network.runner.Spawn(prefab);
            
            newInstance.transform.SetParent(_uiPlaceholder);
            newInstance.GetComponent<GameOverScreen>()?.Setup(serviceLocator.Get<LoadSceneService>());
            return newInstance.gameObject;
        }

        public GameObject CreateProjectile(uint id, Vector3 spawnPoint, Vector3 targetPos)
        {
            if (_heroes.TryGetHeroBy(id, out var hero))
            {
                GameObject prefab = _assetProvider.GetProjectile(hero.type);
                NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity);
                
                newInstance.GetComponent<AttackProjectile>()?.Setup(id, targetPos, serviceLocator.Get<AttackService>());
                return newInstance.gameObject;
            }
            return null;
        }
        
        public GameObject CreateItem(ItemType item, Vector3 spawnPoint)
        {
            GameObject prefab = _assetProvider.GetItem(item);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity);
            
            newInstance.GetComponent<CollectableItem>()?.Setup(item, serviceLocator.Get<ItemsService>());
            return newInstance.gameObject;
        }
        
        public void Dispose() { }
    }
}