using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Configs.Enemies;
using CodeBase.Infrastructure;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class EnemiesModel : IDisposable
    {
        public event Action<uint, float> onHealthChanged;

        private readonly Dictionary<uint, EnemyModel> _enemies = new Dictionary<uint, EnemyModel>();
        private readonly Dictionary<EnemyType, EnemyData> _enemyDataMap = new Dictionary<EnemyType, EnemyData>();

        public EnemiesModel(AssetProvider assetProvider)
        {
            var enemies = assetProvider.GetConfig<EnemiesConfig>()?.enemies;
            if (enemies != null)
            {
                foreach (var enemy in enemies)
                    _enemyDataMap[enemy.type] = enemy;   
            }
        }

        public void Dispose()
        {
            foreach (var enemy in _enemies.Values)
                enemy.onHealthChanged -= OnHealthChanged;

            _enemies.Clear();
        }

        public EnemyModel Add(uint id, EnemyType enemyType)
        {
            var data = GetDataBy(enemyType);
            data.id = id;
            return Add(data);
        }

        public EnemyModel Add(EnemyData data)
        {
            if (_enemies.ContainsKey(data.id))
            {
                Debug.LogError($"{nameof(EnemiesModel)}: Enemy with id:{data.id} already exists");
                return null;
            }

            _enemies[data.id] = new EnemyModel(data);
            _enemies[data.id].onHealthChanged += OnHealthChanged;
            return _enemies[data.id];
        }

        public void Remove(uint id)
        {
            if (_enemies.TryGetValue(id, out var enemy))
            {
                enemy.onHealthChanged -= OnHealthChanged;
                _enemies.Remove(id);
            }
        }

        public EnemyModel GetBy(uint id)
        {
            if (_enemies.TryGetValue(id, out var enemy))
                return enemy;

            Debug.Log($"{nameof(EnemiesModel)}: Enemy with id:{id.ToString()} is not found");
            return null;
        }

        public List<EnemyData> GetAvailableEnemyData() => _enemyDataMap.Values.ToList();

        public void Actualize(EnemyData[] dataList)
        {
            _enemies.Clear();
            var enemies = _enemies.Values.ToArray();
            int count = enemies.Length > dataList.Length ? enemies.Length : dataList.Length;
            for (var i = 0; i < count; i++)
            {
                if (i >= dataList.Length)
                    continue;

                if (i < enemies.Length)
                {
                    enemies[i].Setup(dataList[i]);
                    _enemies[enemies[i].id] = enemies[i];
                }
                else
                {
                    Add(dataList[i]);
                }
            }
        }

        public bool TryGetBy(uint id, out EnemyModel model)
        {
            model = GetBy(id);
            return model != null;
        }

        private EnemyData GetDataBy(EnemyType type)
        {
            if (_enemyDataMap.TryGetValue(type, out var data))
                return data;

            Debug.LogError($"{nameof(EnemiesModel)}: Enemy data for type:{type} not found");
            return default;
        }

        private void OnHealthChanged(uint id)
        {
            if (!_enemies.TryGetValue(id, out var enemy))
                return;
            
            onHealthChanged?.Invoke(id, enemy.currentHealth);
            
            if (enemy.currentHealth <= 0)
                Remove(id);
        }
    }
}