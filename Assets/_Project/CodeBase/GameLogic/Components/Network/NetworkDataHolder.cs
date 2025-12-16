using System.Collections;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Network
{ 
    public abstract class NetworkDataHolder<T> : NetworkBehaviour where T : struct
    {
        private WaitForSeconds _updateInterval;
        private Coroutine _activeRoutine;

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _updateInterval = new WaitForSeconds(Runner.Config.HostMigration.UpdateDelay);
                _activeRoutine = StartCoroutine(ActualizeWithInterval());   
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority && _activeRoutine != null) 
                StopCoroutine(_activeRoutine);
        }

        public abstract T GetData();
        
        protected abstract void ActualizeData();

        private IEnumerator ActualizeWithInterval()
        {
            while (true)
            {
                yield return _updateInterval;
                ActualizeData();
            }
        }
    }
}