using System.Threading.Tasks;
using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase
{
    public class NetworkProvider : IService
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

        private readonly NetworkRunner _runnerPrefab;
        private NetworkRunner _runner;

        public NetworkProvider(NetworkRunner runnerPrefab, NetworkRunnerCallbacks callbacks)
        {
            _runnerPrefab = runnerPrefab;
            this.callbacks = callbacks;
        }

        public void Dispose()
        {
        }

        private NetworkRunner CreateRunner()
        {
            var newRunner = Object.Instantiate(_runnerPrefab);
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