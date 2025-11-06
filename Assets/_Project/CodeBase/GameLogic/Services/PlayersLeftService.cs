using CodeBase.GameLogic.Components.Network;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Services
{
    public class PlayersLeftService : IInitializeService
    {
        private readonly HeroesModel _heroes;
        private readonly NetworkRunnerCallbacks _netCallbacks;
        private readonly HeroesInstanceProvider _instanceProvider;

        public PlayersLeftService(HeroesModel heroes, NetworkRunnerCallbacks netCallbacks,
            HeroesInstanceProvider instanceProvider)
        {
            _heroes = heroes;
            _netCallbacks = netCallbacks;
            _instanceProvider = instanceProvider;
        }
        
        public void Initialize()
        {
            _netCallbacks.onPlayerLeft += OnPlayerLeft;
        }
        
        public void Dispose()
        {
            _netCallbacks.onPlayerLeft -= OnPlayerLeft;
        }
        
        private void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
        {
            if (runner.TryGetPlayerObject(player, out var netObj)) 
                _heroes.Remove(netObj.Id.Raw);
            
            _instanceProvider.Remove(player);
        }
    }
}