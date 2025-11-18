using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroSpawner : NetworkBehaviour
    {
        private PlayerData _playerData;
        private HeroSpawnService _heroSpawnService;
        private HeroesInstanceProvider _instanceProvider;

        public override void Spawned()
        {
            SetupDependencies();
            TrySpawnHero();
        }
        
        private void SetupDependencies()
        {
            _playerData = BehaviourInjector.instance.Resolve<PlayerData>();
            _instanceProvider = BehaviourInjector.instance.Resolve<HeroesInstanceProvider>();
            _heroSpawnService = BehaviourInjector.instance.Resolve<HeroSpawnService>();
        }
        
        private void TrySpawnHero()
        {
            if (!Runner.TryGetPlayerObject(Runner.LocalPlayer, out _)) 
                Rpc_SpawnClientHero(Runner.LocalPlayer, _playerData.chosenHero);
        }
        
        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_SpawnClientHero(PlayerRef player, HeroType heroType)
        {
            _heroSpawnService.SpawnHero(player, heroType);
            _instanceProvider.ActualizeInstances(Runner.GetActivePlayerObjects());
        }
    }
}