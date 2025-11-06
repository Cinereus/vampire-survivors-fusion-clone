using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroSpawner : NetworkBehaviour
    {
        private HeroesModel _heroes;
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
            var services = ServiceLocator.instance;
            _heroes = services.Get<HeroesModel>();
            _playerData = services.Get<PlayerData>();
            _instanceProvider = services.Get<HeroesInstanceProvider>();
            _heroSpawnService = services.Get<HeroSpawnService>();
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
            Rpc_RequestActualization(_heroes.GetAllAsDataList());
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_RequestActualization(HeroData[] dataList)
        {
            if (!HasStateAuthority) 
                _heroes.ActualizeAll(dataList);
            
            _instanceProvider.ActualizeInstances(Runner.GetActivePlayerObjects());
        }
    }
}