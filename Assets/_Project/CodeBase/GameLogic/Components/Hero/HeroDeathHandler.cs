using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : NetworkBehaviour
    {
        [Networked]
        private float currentHealth { get; set; }
        
        private HeroModel _model;
        private ChangeDetector _changeDetector;
        
        public void Setup(HeroModel model)
        {
            _model = model;
        }
        
        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (HasStateAuthority)
            {
                _model.onHealthChanged += OnHealthChanged;
                currentHealth = _model.currentHealth;
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _model.onHealthChanged -= OnHealthChanged;
            }
        }

        public override void Render() => CheckNetworkPropertyChanged();

        private void CheckNetworkPropertyChanged()
        {
            foreach (var propertyName in _changeDetector.DetectChanges(this))
            {
                if (propertyName == nameof(currentHealth) && HasInputAuthority && currentHealth <= 0 ) 
                    ServiceLocator.instance.Get<GameFactory>().CreateGameOverScreenLocal();
            }
        }
        
        private void OnHealthChanged(uint id)
        {
            currentHealth = _model.currentHealth;
            
            if (HasStateAuthority && _model.currentHealth <= 0) 
                Runner.Disconnect(Object.InputAuthority);
        }
    }
}