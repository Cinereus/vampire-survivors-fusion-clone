using CodeBase.GameLogic;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.UI
{
    public class UserHudStatTracker : NetworkBehaviour
    {
        [SerializeField]
        private RectTransform _remoteUserHudPlaceholder;
        
        [Networked] 
        private float currentXp { get; set; }
        
        [Networked] 
        private float currentHealth { get; set; }
        
        [Networked] 
        private float maxXp { get; set; }
        
        [Networked] 
        private float maxHealth { get; set; }

        private ChangeDetector _changeDetector;
        private UserHud _userHud;   
        private HeroesModel _heroes;
        
        public override void Spawned()
        {
            _heroes = ServiceLocator.instance.Get<HeroesModel>();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            SetupProperties();
            
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
            
            Destroy(_userHud.gameObject);
        }

        public override void Render()
        {
            CheckNetworkPropertyChanged();
        }

        private void Start()
        {
            InitUserHudLocal();
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

        private void InitUserHudLocal()
        {
            var factory = ServiceLocator.instance.Get<GameFactory>();
            
            _userHud = HasInputAuthority
                ? factory.CreateClientUserHudLocal()
                : factory.CreateRemoteUserHudLocal(_remoteUserHudPlaceholder);
            
            _userHud?.UpdateXpValue(currentXp, maxXp);
            _userHud?.UpdateHealthValue(currentHealth, maxHealth);
        }
        
        private void CheckNetworkPropertyChanged()
        {
            foreach (var propertyName in _changeDetector.DetectChanges(this)) 
                UpdateProperty(propertyName);
        }

        private void UpdateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(currentXp):
                    _userHud?.UpdateXpValue(currentXp, maxXp);
                    break;
                case nameof(currentHealth):
                    _userHud?.UpdateHealthValue(currentHealth, maxHealth);
                    break;
            }
        }

        private void OnHealthChanged(uint id)
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