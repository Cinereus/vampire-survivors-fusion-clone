using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using Fusion;
using UnityEngine;

namespace CodeBase.UI
{
    public class UserHudStatTracker : NetworkBehaviour
    {
        [SerializeField]
        private UserHud _remoteUserHud;
        
        [Networked] 
        private float currentXp { get; set; }
        
        [Networked] 
        private float currentHealth { get; set; }
        
        [Networked] 
        private float maxXp { get; set; }
        
        [Networked] 
        private float maxHealth { get; set; }

        private HeroesModel _heroes;
        private UIManager _uiManager;
        private UserHudPanel _userHudPanel;
        private ChangeDetector _changeDetector;
        
        public override void Spawned()
        {
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            SetupDependencies();
            SetupProperties();
            InitUserHud();
            UpdateUserHud();
            
            if (HasStateAuthority)
            {
                _heroes.onXpChanged += OnXpChanged; 
                _heroes.onHealthChanged += OnHealthChanged;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _heroes.onXpChanged -= OnXpChanged; 
                _heroes.onHealthChanged -= OnHealthChanged;
            }
            
            if (HasInputAuthority)
                _uiManager.Hide<UserHudPanel>();
            else 
                _remoteUserHud.Hide();
        }

        public override void Render()
        {
            CheckNetworkPropertyChanged();
        }
        
        private void InitUserHud()
        {
            if (HasInputAuthority)
                _userHudPanel = _uiManager.Show<UserHudPanel>();
            else
                _remoteUserHud.Show();
        }
        
        private void SetupDependencies()
        {
            _heroes = BehaviourInjector.instance.Resolve<HeroesModel>();
            _uiManager = BehaviourInjector.instance.Resolve<UIManager>();
        }
        
        private void SetupProperties()
        {
            if (HasStateAuthority && _heroes.TryGetBy(Object.Id.Raw, out var model))
            {
                currentXp = model.currentXp;
                maxXp = model.maxXp;
                currentHealth = model.currentHealth;
                maxHealth = model.maxHealth;    
            }
        }

        private void UpdateUserHud()
        {
            _remoteUserHud?.UpdateXpValue(currentXp, maxXp);
            _remoteUserHud?.UpdateHealthValue(currentHealth, maxHealth);
            _userHudPanel?.UpdateXpValue(currentXp, maxXp);
            _userHudPanel?.UpdateHealthValue(currentHealth, maxHealth);    
        }
        
        private void CheckNetworkPropertyChanged()
        {
            foreach (var propertyName in _changeDetector.DetectChanges(this)) 
                UpdateProperty(propertyName);
        }

        private void UpdateProperty(string propertyName)
        {
            if (propertyName == nameof(currentXp) || propertyName == nameof(currentHealth)) 
                UpdateUserHud();
        }

        private void OnHealthChanged(uint id, float _)
        {
            if (Object.Id.Raw == id && _heroes.TryGetBy(id, out var model))
            {
                maxHealth = model.maxHealth;
                currentHealth = model.currentHealth;    
            }
        }

        private void OnXpChanged(uint id)
        {
            if (Object.Id.Raw == id && _heroes.TryGetBy(id, out var model))
            {
                maxXp = model.maxXp;
                currentXp = model.currentXp;
            }
        }
    }
}