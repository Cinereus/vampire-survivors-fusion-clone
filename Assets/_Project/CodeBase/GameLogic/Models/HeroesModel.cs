using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class HeroesModel : IService
    {
        public event Action<uint> onXpChanged;
        public event Action<uint> onLevelIncreased;
        public event Action<uint> onHealthChanged;
        
        private readonly Dictionary<HeroType, HeroData> _dataMap = new Dictionary<HeroType, HeroData>();
        private readonly Dictionary<uint, HeroModel> _heroes = new Dictionary<uint, HeroModel>();

        public HeroesModel(List<HeroData> heroes)
        {
            foreach (var hero in heroes) 
                _dataMap[hero.heroType] = hero;
        }
        
        public HeroModel GetBy(uint id) => _heroes.GetValueOrDefault(id);

        public bool TryGetBy(uint id, out HeroModel model)
        {
            model = GetBy(id);
            return model != null;
        }

        public HeroModel Add(uint id, HeroType type)
        {
            var data = GetDataBy(type);
            data.id = id;
            return Add(data);
        }
        
        public HeroModel Add(HeroData data)
        {
            if (_heroes.ContainsKey(data.id))
            {
                Debug.LogError($"{nameof(HeroesModel)}: Hero with id:{data.id} already exists");
                return null;
            }
            
            _heroes[data.id] = new HeroModel(data);
            _heroes[data.id].onXpChanged += OnXpChanged;
            _heroes[data.id].onLevelIncreased += OnLevelIncreased;
            _heroes[data.id].onHealthChanged += OnHealthChanged;
            return _heroes[data.id];
        }

        public void Remove(uint id)
        {
            if (_heroes.TryGetValue(id, out var hero))
            {
                hero.onXpChanged -= OnXpChanged;
                hero.onHealthChanged -= OnHealthChanged;
                hero.onLevelIncreased -= OnLevelIncreased;
                _heroes.Remove(id);   
            }
        }

        public HeroData[] GetAllAsDataList() => _heroes.Values.Select(h => h.ToData()).ToArray();
        
        public void ActualizeAll(HeroData[] dataList)
        {
            var heroes = _heroes.Values.ToArray();
            _heroes.Clear();
            
            int count = _heroes.Values.Count > dataList.Length ? heroes.Length : dataList.Length; 
            for (var i = 0; i < count; i++)
            {
                if (i >= dataList.Length)
                    continue;
                
                if (i < heroes.Length)
                {
                    heroes[i].Setup(dataList[i]);
                    _heroes[heroes[i].id] = heroes[i];
                }
                else
                { 
                    Add(dataList[i]);
                }
            }
        }

        public void Dispose()
        {
            foreach (var hero in _heroes.Values)
            {
                hero.onXpChanged -= OnXpChanged;
                hero.onHealthChanged -= OnHealthChanged;
                hero.onLevelIncreased -= OnLevelIncreased;
            }
            _heroes.Clear();
        }

        private HeroData GetDataBy(HeroType type)
        {
            if (_dataMap.TryGetValue(type, out var data))
                return data;

            Debug.LogError($"{nameof(HeroesModel)}: Data for type:{type} not found");
            return default;
        }

        private void OnHealthChanged(uint id)
        {
            if (_heroes.TryGetValue(id, out var hero))
            {
                onHealthChanged?.Invoke(id);
                
                if (hero.currentHealth <= 0)
                    Remove(id);    
            }
        }
        
        private void OnLevelIncreased(uint id)
        {
            if (_heroes.TryGetValue(id, out _)) 
                onLevelIncreased?.Invoke(id);
        }

        private void OnXpChanged(uint id)
        {
            if (_heroes.TryGetValue(id, out _)) 
                onXpChanged?.Invoke(id);
        }
    }
}