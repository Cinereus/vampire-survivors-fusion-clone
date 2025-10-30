using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Attacks;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Components.Hero;
using CodeBase.GameLogic.Models;
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
        private readonly AssetProvider _assetProvider;
        private readonly RectTransform _uiPlaceholder;

        public GameFactory(HeroesModel heroes, EnemiesModel enemies, AssetProvider assetProvider, 
            RectTransform uiPlaceholder, NetworkContainer network)
        {
            _heroes = heroes;
            _enemies = enemies;
            _network = network;
            _assetProvider = assetProvider;
            _uiPlaceholder = uiPlaceholder;
        }

        public GameObject CreateEnemy(EnemyType type, Vector3 spawnPoint)
        {
            if (!_network.runner.IsServer)
                return null;
            
            GameObject prefab = _assetProvider.GetEnemy(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity, 
                onBeforeSpawned: (_, netObject) => 
                {
                    EnemyModel model = _enemies.Add(netObject.Id.Raw, type);
                    
                    netObject.GetComponent<HeroChaser>()?.Setup(model.speed);
                    netObject.GetComponent<LootSpawner>()?.Setup(model);
                    netObject.GetComponent<EnemyMeleeAttack>()?.Setup(model.id, model.attackCooldown);
                    netObject.GetComponent<EnemyDeathHandler>()?.Setup(model);
                });

            return newInstance.gameObject;
        }

        public NetworkObject CreateHero(HeroType type, PlayerRef player,  Vector2 spawnPoint)
        {
            if (!_network.runner.IsServer)
                return null;
            
            GameObject prefab = _assetProvider.GetHero(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity, player, 
                onBeforeSpawned: (_, netObject) =>
                {
                    HeroModel model = _heroes.Add(netObject.Id.Raw, type);
                    
                    netObject.GetComponent<PlayerMovement>()?.Setup(model);
                    netObject.GetComponent<HeroRemoteAttack>()?.Setup(model);
                    netObject.GetComponent<HeroDeathHandler>()?.Setup(model);
                    netObject.GetComponent<UserHudStatTracker>()?.Setup(model);
                });
            
            return newInstance;
        }
        
        public UserHud CreateClientUserHudLocal()
        {
            GameObject prefab = _assetProvider.GetClientUserHud();
            UserHud newInstance = Object.Instantiate(prefab, _uiPlaceholder).GetComponent<UserHud>();
            return newInstance;
        }
        
        public UserHud CreateRemoteUserHudLocal(RectTransform parent)
        {
            GameObject prefab = _assetProvider.GetRemoteUserHud();
            UserHud newInstance = Object.Instantiate(prefab, parent).GetComponent<UserHud>(); 
            
            return newInstance;
        }
        
        public GameOverScreen CreateGameOverScreenLocal()
        {
            GameObject prefab = _assetProvider.GetGameOverScreen();
            GameOverScreen newInstance = Object.Instantiate(prefab, _uiPlaceholder)
                .GetComponent<GameOverScreen>();
            newInstance?.Setup();
            
            return newInstance;
        }

        public GameObject CreateProjectile(uint id, Vector3 spawnPoint, Vector3 targetPos)
        {
            if (!_network.runner.IsServer)
                return null;
            
            if (_heroes.TryGetHeroBy(id, out var hero))
            {
                GameObject prefab = _assetProvider.GetProjectile(hero.type);
                NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity, 
                    onBeforeSpawned: (_, netObject) =>
                    {
                        netObject.GetComponent<AttackProjectile>()?.Setup(id, targetPos); 
                    });
                
                return newInstance.gameObject;
            }
            return null;
        }
        
        public GameObject CreateItem(ItemType item, Vector3 spawnPoint)
        {
            if (!_network.runner.IsServer)
                return null;
            
            GameObject prefab = _assetProvider.GetItem(item);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, Quaternion.identity, 
                onBeforeSpawned: (_, netObject) =>
                {
                    netObject.GetComponent<CollectableItem>()?.Setup(item);        
                });
            
            return newInstance.gameObject;
        }
        
        public GameObject CreateHeroSpawner(PlayerRef playerRef)
        {
            Debug.Log($"Spawn call by: IsServer={_network.runner.IsServer}, IsClient={_network.runner.IsClient}, GameMode={_network.runner.GameMode}");
            if (!_network.runner.IsServer)
                return null;
            
            GameObject prefab = _assetProvider.GetHeroSpawner();
            NetworkObject newInstance =
                _network.runner.Spawn(prefab, Vector3.zero, Quaternion.identity, playerRef, onBeforeSpawned:
                    (r, o) => Debug.Log($"BeforeSpawn: owner={o.InputAuthority}") );
            
            return newInstance.gameObject;
        }
        
        public GameObject CreateEnemySpawner()
        {
            Debug.Log($"Spawn call by: IsServer={_network.runner.IsServer}, IsClient={_network.runner.IsClient}, GameMode={_network.runner.GameMode}");
            if (!_network.runner.IsServer)
                return null;
            
            Debug.Log("Spawn enemy spawner made");
            GameObject prefab = _assetProvider.GetEnemySpawner();
            NetworkObject newInstance =
                _network.runner.Spawn(prefab, Vector3.zero, Quaternion.identity);
            
            return newInstance.gameObject;
        }
        
        public void Dispose() { }
    }
}