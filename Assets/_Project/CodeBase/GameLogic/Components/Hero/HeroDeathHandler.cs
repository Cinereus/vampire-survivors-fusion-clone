using System;
using CodeBase.GameLogic.Models;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : MonoBehaviour
    {
        private HeroModel _model;

        public void Setup(HeroModel model)
        {
            _model = model;
            model.onHealthChanged += OnHealthChanged;
        }

        private void OnDestroy() => _model.onHealthChanged -= OnHealthChanged;
        
        private void OnHealthChanged(Guid _)
        {
            if (_model.currentHealth <= 0) 
                Destroy(gameObject);
        }
    }
}