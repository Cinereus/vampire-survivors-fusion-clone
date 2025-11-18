using Fusion;
using System;
using System.Linq;
using VContainer.Unity;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Network;

namespace CodeBase.GameLogic.Services
{
    public class HostMigrationService : IStartable, IDisposable
    {
        private readonly NetworkRunnerCallbacks _callbacks;
        private readonly MatchmakingService _matchmakingService;
        private readonly NetObjectRestoreService _restoreService;
        private readonly HeroesInstanceProvider _instanceProvider;

        public HostMigrationService(MatchmakingService matchmakingService, NetObjectRestoreService restoreService,
            NetworkProvider network, HeroesInstanceProvider instanceProvider)
        {
            _callbacks = network.callbacks;
            _restoreService = restoreService;
            _matchmakingService = matchmakingService;
            _instanceProvider = instanceProvider;
        }

        public void Start()
        {
            _callbacks.onHostMigration += OnHostMigration;
        }

        public void Dispose()
        {
            _callbacks.onHostMigration -= OnHostMigration;
        }

        private void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
        {
            _instanceProvider.Clear();

            foreach (var obj in runner.GetAllNetworkObjects())
                runner.Despawn(obj);

            _matchmakingService.MigrateGameSession(token, OnMigrationResume);
        }

        private void OnMigrationResume(NetworkRunner runner)
        {
            var snapshot = runner.GetResumeSnapshotNetworkObjects().ToList();
            foreach (var netObj in snapshot)
                RestoreState(netObj, runner);

            _instanceProvider.ActualizeInstances(runner.GetActivePlayerObjects());
        }

        private void RestoreState(NetworkObject netObj, NetworkRunner runner)
        {
            if (netObj.TryGetBehaviour<HeroDataHolder>(out var heroData) &&
                runner.ActivePlayers.Contains(netObj.InputAuthority))
            {
                _restoreService.RestoreHero(netObj, heroData.GetData());
                return;
            }

            if (netObj.TryGetBehaviour<EnemyDataHolder>(out var enemyData))
            {
                _restoreService.RestoreEnemy(netObj, enemyData.GetData());
                return;
            }

            if (netObj.TryGetBehaviour<CollectableItem>(out var item))
            {
                _restoreService.RestoreItem(netObj, item.itemType);
                return;
            }
        }
    }
}