using System;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class LootSpawner : MonoBehaviour
    {
        private EnemyModel _model;
        private LootSpawnService _lootService;

        public void Setup(EnemyModel model, LootSpawnService lootService)
        {
            _model = model;
            _lootService = lootService;
            _model.onDeath += OnDeath;
        }

        private void OnDestroy() => _model.onDeath -= OnDeath;
        
        private void OnDeath(Guid obj)
        {
            _lootService.SpawnXp(_model.type, transform.position);
            _lootService.SpawnHealPotion(_model.lootProbability, transform.position);
        }
    }
}