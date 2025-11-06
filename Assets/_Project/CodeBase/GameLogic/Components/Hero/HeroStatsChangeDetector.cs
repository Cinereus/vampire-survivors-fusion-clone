using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroStatsChangeDetector : NetworkBehaviour
    {
        [Networked] 
        private float currentHealth { get; set; } 
        
        private ChangeDetector _changeDetector;
        private HeroesModel _heroes;

        public override void Spawned()
        {
            _heroes = ServiceLocator.instance.Get<HeroesModel>();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (HasStateAuthority && _heroes.TryGetBy(Object.Id.Raw, out _))
            {
                _heroes.onLevelIncreased += OnLevelIncreased;
            }
        }

        public override void Render() => 
            CheckPropertyChanged();

        private void CheckPropertyChanged()
        {
            foreach (var propertyName in _changeDetector.DetectChanges(this)) 
                TrySaveLocalState(propertyName);
        }

        private void TrySaveLocalState(string propertyName)
        {
            if (propertyName == nameof(currentHealth) && _heroes.TryGetBy(Object.Id.Raw, out var model))
                model.currentHealth = currentHealth;
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