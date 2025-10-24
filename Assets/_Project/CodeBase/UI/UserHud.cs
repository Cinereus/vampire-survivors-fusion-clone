using System;
using CodeBase.GameLogic.Models;
using UnityEngine;

namespace CodeBase.UI
{
    public class UserHud : MonoBehaviour
    {
        [SerializeField] 
        private ProgressBar _healthBar;

        [SerializeField] 
        private ProgressBar _xpBar;

        private Guid _id;
        private HeroModel _model;

        public void Setup(HeroModel model)
        {
            _model = model;
            UpdateXpValue();
            UpdateHealthValue();
            _model.onXpChanged += OnXpChanged; 
            _model.onHealthChanged += OnHealthChanged;
        }

        private void OnDestroy()
        {
            if (_model != null)
            { 
                _model.onXpChanged -= OnXpChanged; 
                _model.onHealthChanged -= OnHealthChanged;
            }
        }

        private void OnXpChanged(Guid _) => UpdateXpValue();
        
        private void OnHealthChanged(Guid _) => UpdateHealthValue();

        private void UpdateHealthValue() =>
            _healthBar.SetProgress($"{_model.currentHealth}/{_model.maxHealth}", _model.currentHealth,
                _model.maxHealth);

        private void UpdateXpValue() => 
            _xpBar.SetProgress($"{_model.currentXP}/{_model.maxXP}", _model.currentXP, _model.maxXP);
    }
}