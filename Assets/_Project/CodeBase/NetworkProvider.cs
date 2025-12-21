using System.Threading.Tasks;
using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure.AssetManagement;
using Fusion;
using UnityEngine;

namespace CodeBase
{
    public class NetworkProvider
    {
        public NetworkRunner runner
        {
            get
            {
                if (!_runner) 
                    _runner = CreateRunner();

                return _runner;
            }
        }

        public NetworkRunnerCallbacks callbacks { get; private set; }
        
        private readonly AssetProvider _assetProvider;
        private NetworkRunner _runner;

        public NetworkProvider(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public void Initialize()
        {
            callbacks = Object.Instantiate(_assetProvider.GetNetworkCallbacks())
                .GetComponent<NetworkRunnerCallbacks>();
        }

        private NetworkRunner CreateRunner()
        {
            var newRunner = Object.Instantiate(_assetProvider.GetNetworkRunner()).GetComponent<NetworkRunner>();
            newRunner.AddCallbacks(callbacks);
            return newRunner;
        }

        public async Task ClearCurrentRunner(ShutdownReason reason = ShutdownReason.Ok)
        {
            runner.ProvideInput = false;
            runner.RemoveCallbacks(callbacks);
            await runner.Shutdown(shutdownReason: reason);
            _runner = null;
        }
    }
}