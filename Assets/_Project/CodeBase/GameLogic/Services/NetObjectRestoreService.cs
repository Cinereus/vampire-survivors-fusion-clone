using System;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Models;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class NetObjectRestoreService
    {
        private readonly Heroes _heroes;
        private readonly Enemies _enemies;
        private readonly NetworkProvider _network;

        public NetObjectRestoreService(Heroes heroes, Enemies enemies, NetworkProvider network)
        {
            _heroes = heroes;
            _enemies = enemies;
            _network = network;
        }

        public void RestoreHero(NetworkObject snapshot, HeroData restoredData)
        {
            RestoreNetObject(snapshot,
                onBeforeSpawned: netObject =>
                { 
                    _heroes.Add(netObject.Id.Raw, restoredData);
                    _network.runner.SetPlayerObject(netObject.InputAuthority, netObject);
                });
            Debug.Log($"Hero with id {snapshot.Id.Raw} restored.");
        }

        public void RestoreEnemy(NetworkObject snapshot, EnemyData restoredData)
        {
            RestoreNetObject(snapshot, onBeforeSpawned: netObject => _enemies.Add(netObject.Id.Raw, restoredData));
            Debug.Log($"Enemy with id {snapshot.Id.Raw} restored.");
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