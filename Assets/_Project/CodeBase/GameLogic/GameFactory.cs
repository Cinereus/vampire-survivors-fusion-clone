using System;
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
using Object = UnityEngine.Object;

namespace CodeBase.GameLogic
{
    public class GameFactory : IService
    {
        private readonly HeroesModel _heroes;
        private readonly EnemiesModel _enemies;
        private readonly NetworkProvider _network;
        private readonly AssetProvider _assetProvider;
        private readonly RectTransform _uiPlaceholder;

        public GameFactory(HeroesModel heroes, EnemiesModel enemies, AssetProvider assetProvider,
            RectTransform uiPlaceholder, NetworkProvider network)
        {
            _heroes = heroes;
            _enemies = enemies;
            _network = network;
            _assetProvider = assetProvider;
            _uiPlaceholder = uiPlaceholder;
        }

        public NetworkObject RestoreHero(NetworkObject snapshot)
        {
            NetworkObject newInstance = RestoreNetObject(snapshot);
            _network.runner.SetPlayerObject(snapshot.InputAuthority, newInstance);
            Debug.Log($"Hero with id {snapshot.Id.Raw} restored.");
            return newInstance;
        }
        
        public NetworkObject RestoreEnemy(NetworkObject snapshot)
        {
            NetworkObject newInstance = RestoreNetObject(snapshot, onBeforeSpawned: netObj =>
                {
                    var model = _enemies.GetBy(snapshot.Id.Raw);
                    netObj.GetComponent<HeroChaser>()?.Setup(model.speed);
                    netObj.GetComponent<EnemyMeleeAttack>()?.Setup(model.id, model.attackCooldown);
                });
            
            Debug.Log($"enemy with id {snapshot.Id.Raw} restored.");
            return newInstance;
        }
        
        public NetworkObject RestoreItem(NetworkObject snapshot, ItemType itemType)
        { 
            NetworkObject newInstance = RestoreNetObject(snapshot, 
                onBeforeSpawned: netObj => netObj.GetComponent<CollectableItem>()?.Setup(itemType));
            
            Debug.Log($"Item type of {itemType} with id {snapshot.Id.Raw} restored.");
            return newInstance;    
        }
        
        public NetworkObject CreateEnemy(EnemyType type, Vector3 spawnPoint, Quaternion rotation)
        {
            if (!_network.runner.IsServer)
                return null;

            GameObject prefab = _assetProvider.GetEnemy(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, rotation,
                onBeforeSpawned: (_, netObject) =>
                {
                    EnemyModel model = _enemies.Add(netObject.Id.Raw, type);
                    netObject.GetComponent<HeroChaser>()?.Setup(model.speed);
                    netObject.GetComponent<EnemyMeleeAttack>()?.Setup(model.id, model.attackCooldown);
                });

            return newInstance;
        }
        
        public NetworkObject CreateHero(HeroType type, PlayerRef player, Vector2 spawnPoint, Quaternion rotation)
        {
            if (!_network.runner.IsServer)
                return null;

            GameObject prefab = _assetProvider.GetHero(type);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, rotation, inputAuthority: player,
                onBeforeSpawned: (_, netObject) => _heroes.Add(netObject.Id.Raw, type));
            _network.runner.SetPlayerObject(player, newInstance);
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

        public GameOverScreen CreateGameOverScreenLocal(GameObject loadingScreen)
        {
            GameObject prefab = _assetProvider.GetGameOverScreen();
            GameOverScreen newInstance = Object.Instantiate(prefab, _uiPlaceholder).GetComponent<GameOverScreen>();
            newInstance?.Setup(loadingScreen);
            return newInstance;
        }

        public GameObject CreateProjectile(uint id, Vector3 spawnPoint, Vector3 targetPos)
        {
            if (!_network.runner.IsServer)
                return null;

            if (_heroes.TryGetBy(id, out var hero))
            {
                GameObject prefab = _assetProvider.GetProjectile(hero.type);
                NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint,
                    onBeforeSpawned: (_, netObject) => netObject.GetComponent<AttackProjectile>()?.Setup(id, targetPos));

                return newInstance.gameObject;
            }
            return null;
        }

        public GameObject CreateItem(ItemType item, Vector3 spawnPoint)
        {
            if (!_network.runner.IsServer)
                return null;

            GameObject prefab = _assetProvider.GetItem(item);
            NetworkObject newInstance = _network.runner.Spawn(prefab, spawnPoint, 
                onBeforeSpawned: (_, netObject) => netObject.GetComponent<CollectableItem>()?.Setup(item));

            return newInstance.gameObject;
        }

        public HeroSpawner CreateHeroSpawner()
        {
            if (!_network.runner.IsServer)
                return null;

            GameObject prefab = _assetProvider.GetHeroSpawner();
            NetworkObject newInstance = _network.runner.Spawn(prefab);
            
            return newInstance.GetComponent<HeroSpawner>();
        }

        public GameObject CreateEnemySpawner()
        {
            if (!_network.runner.IsServer)
                return null;

            GameObject prefab = _assetProvider.GetEnemySpawner();
            NetworkObject newInstance = _network.runner.Spawn(prefab);
            return newInstance.gameObject;
        }

        public void Dispose()
        {
        }
        
        private NetworkObject RestoreNetObject(NetworkObject snapshot, Action<NetworkObject> onBeforeSpawned = null)
        {
            if (!_network.runner.IsServer)
                return null;

            var pos = Vector2.zero;
            var rot = Quaternion.identity;
            if (snapshot.TryGetBehaviour<NetworkTRSP>(out var posInfo))
            {
                pos = posInfo.Data.Position;
                rot = posInfo.Data.Rotation;
            }
            
            NetworkObject newInstance = _network.runner.Spawn(snapshot, pos, rot,
                snapshot.InputAuthority, (_, instance) =>
                {
                    instance.CopyStateFrom(snapshot);
                    onBeforeSpawned?.Invoke(instance);
                });
            
            return newInstance;
        }
    }
}