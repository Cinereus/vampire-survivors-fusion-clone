using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroSpawner : NetworkBehaviour
    {
        private PlayerData _playerData;
        private HeroSpawnService _heroSpawnService;
        
        public override void Spawned()
        {
            var services = ServiceLocator.instance;
            _playerData = services.Get<PlayerData>();
            _heroSpawnService = services.Get<HeroSpawnService>();
            Rpc_RequestSpawnHero(Runner.LocalPlayer, _playerData.chosenHero);
        }

        [Rpc(RpcSources.All, RpcTargets.StateAuthority, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_RequestSpawnHero(PlayerRef player, HeroType heroType) =>
            _heroSpawnService.SpawnHero(player, heroType);
    }
}