using System;
using CodeBase.Configs;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Attacks;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Models;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class NetObjectRestoreService
    {
        private readonly EnemiesModel _enemies;
        private readonly NetworkProvider _network;

        public NetObjectRestoreService(EnemiesModel enemies, NetworkProvider network)
        {
            _enemies = enemies;
            _network = network;
        }

        public void RestoreHero(NetworkObject snapshot)
        {
            RestoreNetObject(snapshot,
                onBeforeSpawned: netObject => _network.runner.SetPlayerObject(netObject.InputAuthority, netObject));

            Debug.Log($"Hero with id {snapshot.Id.Raw} restored.");
        }

        public void RestoreEnemy(NetworkObject snapshot)
        {
            RestoreNetObject(snapshot,
                onBeforeSpawned: netObject =>
                {
                    EnemyModel model = _enemies.GetBy(netObject.Id.Raw);
                    netObject.GetComponent<HeroChaser>()?.Initialize(model.speed);
                    netObject.GetComponent<EnemyMeleeAttack>()?.Initialize(model.id, model.attackCooldown);
                });

            Debug.Log($"enemy with id {snapshot.Id.Raw} restored.");
        }

        public void RestoreItem(NetworkObject snapshot, ItemType itemType)
        {
            RestoreNetObject(snapshot,
                onBeforeSpawned: netObject => netObject.GetComponent<CollectableItem>()?.Initialize(itemType));

            Debug.Log($"Item type of {itemType} with id {snapshot.Id.Raw} restored.");
        }

        private void RestoreNetObject(NetworkObject snapshot, Action<NetworkObject> onBeforeSpawned = null)
        {
            if (!_network.runner.IsServer)
                return;

            var pos = Vector2.zero;
            var rot = Quaternion.identity;
            if (snapshot.TryGetBehaviour<NetworkTRSP>(out var posInfo))
            {
                pos = posInfo.Data.Position;
                rot = posInfo.Data.Rotation;
            }

            _network.runner.Spawn(snapshot, pos, rot, snapshot.InputAuthority,
                onBeforeSpawned: (_, netObject) =>
                {
                    netObject.CopyStateFrom(snapshot);
                    onBeforeSpawned?.Invoke(netObject);
                });
        }
    }
}