using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase
{
    public class NetworkContainer : IService
    {
        public NetworkRunner runner { get; }

        public NetworkRunnerCallbacks callbacks { get; }

        public NetworkContainer(NetworkRunner runner, NetworkRunnerCallbacks callbacks)
        {
            this.runner = runner;
            this.callbacks = callbacks;
        }
        
        public void Dispose() { }
    }
}