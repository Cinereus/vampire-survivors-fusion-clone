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
        
        private HeroModel _model;
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
                _model.onXpChanged += OnXpChanged; 
                _model.onHealthChanged += OnHealthChanged;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _model.onXpChanged -= OnXpChanged; 
                _model.onHealthChanged -= OnHealthChanged;
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
            _model = BehaviourInjector.instance.Resolve<Heroes>().GetBy(Object.Id.Raw);
            _uiManager = BehaviourInjector.instance.Resolve<UIManager>();
        }
        
        private void SetupProperties()
        {
            if (HasStateAuthority)
            {
                currentXp = _model.currentXp;
                maxXp = _model.maxXp;
                currentHealth = _model.currentHealth;
                maxHealth = _model.maxHealth;    
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
            if (propertyName is nameof(currentXp) or nameof(currentHealth)) 
                UpdateUserHud();
        }

        private void OnHealthChanged()
        { 
            maxHealth = _model.maxHealth;
            currentHealth = _model.currentHealth;
        }

        private void OnXpChanged()
        {
            maxXp = _model.maxXp;
            currentXp = _model.currentXp;
        }
    }
}