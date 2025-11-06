using System.Linq;
using CodeBase.GameLogic.Components;
using CodeBase.GameLogic.Components.Network;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Services
{
    public class HostMigrationService : IInitializeService
    {
        private readonly GameFactory _factory;
        private readonly HeroesModel _heroes;
        private readonly EnemiesModel _enemies;
        private readonly NetworkRunnerCallbacks _callbacks;
        private readonly MatchmakingService _matchmakingService;
        private readonly HeroesInstanceProvider _instanceProvider;

        public HostMigrationService(MatchmakingService matchmakingService, GameFactory factory,
            NetworkRunnerCallbacks callbacks, HeroesModel heroes, EnemiesModel enemies,
            HeroesInstanceProvider instanceProvider)
        {
            _factory = factory;
            _heroes = heroes;
            _enemies = enemies;
            _callbacks = callbacks;
            _matchmakingService = matchmakingService;
            _instanceProvider = instanceProvider;
        }

        public void Initialize()
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
             if (_heroes.TryGetBy(netObj.Id.Raw, out _)) 
             {
                 if (runner.ActivePlayers.Contains(netObj.InputAuthority))
                    _factory.RestoreHero(netObj);
                 else
                    _heroes.Remove(netObj.Id.Raw);
                 
                 return; 
             }

             if (_enemies.TryGetBy(netObj.Id.Raw, out _))
             {
                 _factory.RestoreEnemy(netObj);
                 return;
             }

             if (netObj.TryGetBehaviour<CollectableItem>(out var item))
             {
                 _factory.RestoreItem(netObj, item.itemType);
                 return;
             }
        }
    }
}