using System;
using CodeBase.GameLogic.Components.Network;
using CodeBase.GameLogic.Models;
using Fusion;
using VContainer.Unity;

namespace CodeBase.GameLogic.Services
{
    public class PlayersLeftService : IInitializable, IDisposable
    {
        private readonly Heroes _heroes;
        private readonly NetworkRunnerCallbacks _netCallbacks;
        private readonly HeroesInstanceProvider _instanceProvider;

        public PlayersLeftService(Heroes heroes, NetworkProvider networkProvider,
            HeroesInstanceProvider instanceProvider)
        {
            _heroes = heroes;
            _netCallbacks = networkProvider.callbacks;
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
            _instanceProvider.Remove(player);
            
            if (runner.TryGetPlayerObject(player, out var netObj))
            {
                _heroes.Remove(netObj.Id.Raw);
                runner.Despawn(netObj);
            }
        }
    }
}