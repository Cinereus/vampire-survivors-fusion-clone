using System;
using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class HeroesModel : IService
    {
        public event Action<HeroModel> onHealthChanged;
        
        private readonly Dictionary<HeroType, HeroData> _dataMap = new Dictionary<HeroType, HeroData>();
        private readonly Dictionary<Guid, HeroModel> _heroes = new Dictionary<Guid, HeroModel>();

        public HeroesModel(HeroesConfig config)
        {
            foreach (var hero in config.heroes) 
                _dataMap[hero.heroType] = hero;
        }

        public HeroModel GetHeroBy(Guid id) => _heroes.GetValueOrDefault(id);

        public bool TryGetHeroBy(Guid id, out HeroModel model)
        {
            model = GetHeroBy(id);
            return model != null;
        }
        
        public HeroModel Add(Guid id, HeroType type)
        {
            if (_heroes.ContainsKey(id))
            {
                Debug.LogError($"{nameof(HeroesModel)}: Hero with id:{id} already exists");
                return null;
            }
            
            _heroes[id] = new HeroModel(id, GetDataBy(type));
            _heroes[id].onHealthChanged += OnHealthChanged;
            return _heroes[id];
        }
        
        public void Dispose() { }

        private HeroData GetDataBy(HeroType type)
        {
            if (_dataMap.TryGetValue(type, out var data))
                return data;
            
            Debug.LogError($"{nameof(HeroesModel)}: Data for type:{type} not found");
            return default;
        }
        
        private void OnHealthChanged(Guid id)
        {
            onHealthChanged?.Invoke(_heroes[id]);
            if (_heroes[id].currentHealth <= 0)
            {
                _heroes[id].onHealthChanged -= OnHealthChanged;
                _heroes.Remove(id);    
            }
        }
    }
}