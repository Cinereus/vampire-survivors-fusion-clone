using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroStatsChangeDetector : NetworkBehaviour
    {
        private HeroesModel _heroes;
        
        public override void Spawned()
        {
            _heroes = BehaviourInjector.instance.Resolve<HeroesModel>();
            
            if (HasStateAuthority && _heroes.TryGetBy(Object.Id.Raw, out _))
            {
                _heroes.onHealthChanged += OnHealthChanged;
                _heroes.onLevelIncreased += OnLevelIncreased;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority && _heroes.TryGetBy(Object.Id.Raw, out _))
            {
                _heroes.onHealthChanged -= OnHealthChanged;
                _heroes.onLevelIncreased -= OnLevelIncreased;
            }
        }

        private void OnHealthChanged(uint id, float _)
        {
            if (Object.Id.Raw == id && _heroes.TryGetBy(id, out var model))
                Rpc_RequestLocalStateSaving(model.ToData());
        }

        private void OnLevelIncreased(uint id)
        {
            if (Object.Id.Raw == id && _heroes.TryGetBy(id, out var model))
                Rpc_RequestLocalStateSaving(model.ToData());
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_RequestLocalStateSaving(HeroData data)
        {
            if (!HasStateAuthority && Object.Id.Raw == data.id && _heroes.TryGetBy(Object.Id.Raw, out var model))
            {
                model.Setup(data);
                Debug.Log($"{nameof(HeroStatsChangeDetector)}: Stats for hero {data.id} saved");
            }
        }
    }
}