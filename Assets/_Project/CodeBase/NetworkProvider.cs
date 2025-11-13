using System.Threading.Tasks;
using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure;
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

        public NetworkRunnerCallbacks callbacks { get; }

        private readonly GameObject _runnerPrefab;
        private NetworkRunner _runner;

        public NetworkProvider(AssetProvider assetProvider)
        {
            _runnerPrefab = assetProvider.GetNetworkRunnerPrefab();
            callbacks = Object.Instantiate(assetProvider.GetNetworkRunnerCallbacksPrefab())
                .GetComponent<NetworkRunnerCallbacks>();
        }

        private NetworkRunner CreateRunner()
        {
            var newRunner = Object.Instantiate(_runnerPrefab).GetComponent<NetworkRunner>();
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