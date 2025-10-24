using System;
using CodeBase.GameLogic.Models;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class HeroDeathHandler : MonoBehaviour
    {
        private HeroModel _model;
        private GameFactory _gameFactory;

        public void Setup(HeroModel model, GameFactory gameFactory)
        {
            _model = model;
            _gameFactory = gameFactory;
            model.onHealthChanged += OnHealthChanged;
        }

        private void OnDestroy() => _model.onHealthChanged -= OnHealthChanged;
        
        private void OnHealthChanged(Guid _)
        {
            if (_model.currentHealth <= 0)
            {
                _gameFactory.CreateGameOverScreen();
                Destroy(gameObject);
            } 
        }
    }
}