using System;
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
        private HeroModel _model;

        public void Setup(HeroModel model)
        {
            _model = model;
        }

        public override void Spawned()
        {
            InitProperties();
            _changeDetector = GetChangeDetector(ChangeDetector.Source.SimulationState);
            
            if (HasStateAuthority)
            { 
                _model.onXpChanged += OnXpChanged; 
                _model.onHealthChanged += OnHealthChanged;
            }
        }

        private void Start()
        {
            InitUserHudLocal();
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _model.onXpChanged -= OnXpChanged; 
                _model.onHealthChanged -= OnHealthChanged;
            }
            
            Destroy(_userHud);
        }

        public override void Render()
        {
            CheckNetworkPropertyChanged();
        }

        private void InitProperties()
        {
            if (!HasStateAuthority) 
                return;
            
            currentXp = _model.currentXp;
            maxXp = _model.maxXp;
            currentHealth = _model.currentHealth;
            maxHealth = _model.maxHealth;
        }

        private void InitUserHudLocal()
        {
            var factory = ServiceLocator.instance.Get<GameFactory>();
            _userHud = HasInputAuthority// && Runner.LocalPlayer == Object.InputAuthority
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

        private void OnHealthChanged(uint _)
        {
            maxHealth = _model.maxHealth;
            currentHealth = _model.currentHealth;
        }

        private void OnXpChanged(uint _)
        {
            maxXp = _model.maxXp;
            currentXp = _model.currentXp;
        }
    }
}