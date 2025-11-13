using System;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Attacks;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic
{
    public class GameFactory
    {
        private readonly HeroesModel _heroes;
        private readonly EnemiesModel _enemies;
        private readonly AssetProvider _assets;
        private readonly NetworkProvider _network;

        public GameFactory(HeroesModel heroes, EnemiesModel enemies, AssetProvider assets, NetworkProvider network)
        {
            _heroes = heroes;
            _enemies = enemies;
            _assets = assets;
            _network = network;
        }

        public NetworkObject CreateEnemy(EnemyType type, Vector3 spawnPoint)
        {
            GameObject prefab = _assets.GetEnemy(type);
            return CreateNetObject(prefab, spawnPoint, Quaternion.identity,
                onBeforeSpawned: netObject =>
                {
                    EnemyModel model = _enemies.Add(netObject.Id.Raw, type);
                    netObject.GetComponent<HeroChaser>()?.Initialize(model.speed);
                    netObject.GetComponent<EnemyMeleeAttack>()?.Initialize(model.id, model.attackCooldown);
                });
        }

        public NetworkObject CreateHero(HeroType type, PlayerRef player, Vector2 spawnPoint)
        {
            GameObject prefab = _assets.GetHero(type);
            return CreateNetObject(prefab, spawnPoint, Quaternion.identity, player,
                onBeforeSpawned: netObject =>
                {
                    _heroes.Add(netObject.Id.Raw, type);
                    _network.runner.SetPlayerObject(player, netObject);
                });
        }

        public NetworkObject CreateProjectile(uint id, Vector3 spawnPoint, Vector3 target)
        {
            if (_heroes.TryGetBy(id, out var hero))
            {
                GameObject prefab = _assets.GetProjectile(hero.type);
                return CreateNetObject(prefab, spawnPoint, Quaternion.identity,
                    onBeforeSpawned: netObject => netObject.GetComponent<AttackProjectile>()?.Initialize(id, target));
            }

            return null;
        }

        public NetworkObject CreateItem(ItemType item, Vector3 spawnPoint)
        {
            GameObject prefab = _assets.GetItem(item);
            return CreateNetObject(prefab, spawnPoint, Quaternion.identity,
                onBeforeSpawned: netObject => netObject.GetComponent<CollectableItem>()?.Initialize(item));
        }

        public NetworkObject CreateHeroSpawner()
        {
            GameObject prefab = _assets.GetHeroSpawner();
            return CreateNetObject(prefab, Vector3.zero, Quaternion.identity);
        }

        public NetworkObject CreateEnemySpawner()
        {
            GameObject prefab = _assets.GetEnemySpawner();
            return CreateNetObject(prefab, Vector3.zero, Quaternion.identity);
        }

        private NetworkObject CreateNetObject(GameObject prefab, Vector3 position, Quaternion rotation,
            PlayerRef? inputAuthority = null, Action<NetworkObject> onBeforeSpawned = null)
        {
            if (!_network.runner.IsServer)
                return null;

            NetworkObject newInstance = _network.runner.Spawn(prefab, position, rotation, inputAuthority,
                onBeforeSpawned: (_, netObject) => { onBeforeSpawned?.Invoke(netObject); });

            return newInstance;
        }
    }
}