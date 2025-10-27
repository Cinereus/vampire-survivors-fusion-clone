using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Configs.Enemies;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class EnemiesModel : IService
    {
        private readonly Dictionary<EnemyType, EnemyData> _enemyDataMap = new Dictionary<EnemyType, EnemyData>();
        private readonly Dictionary<uint, EnemyModel> _enemies = new Dictionary<uint, EnemyModel>();

        public EnemiesModel(EnemiesConfig config)
        {
            foreach (var enemy in config.enemies) 
                _enemyDataMap[enemy.type] = enemy;
        }

        public void Dispose() { }
        
        public EnemyModel Add(uint id, EnemyType enemyType)
        {
            if (_enemies.ContainsKey(id))
            {
                Debug.LogError($"{nameof(EnemiesModel)}: Enemy with id:{id} already exists");
                return null;
            }
            
            _enemies[id] = new EnemyModel(id, GetDataBy(enemyType));
            return _enemies[id];
        }
        
        public EnemyModel GetEnemyBy(uint id)
        {
            if (_enemies.TryGetValue(id, out var enemy))
                return enemy;
            
            Debug.LogError($"{nameof(EnemiesModel)}: Enemy with id:{id.ToString()} is not found");
            return null;
        }

        public List<EnemyData> GetAvailableEnemyData() => _enemyDataMap.Values.ToList();
        
        private EnemyData GetDataBy(EnemyType type)
        {
            if (_enemyDataMap.TryGetValue(type, out var data))
                return data;
            
            Debug.LogError($"{nameof(EnemiesModel)}: Enemy data for type:{type} not found");
            return default;
        }
    }
}