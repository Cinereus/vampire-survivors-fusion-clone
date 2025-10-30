using System.Collections.Generic;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Models
{
    public class HeroesModel : IService
    {
        private readonly Dictionary<HeroType, HeroData> _dataMap = new Dictionary<HeroType, HeroData>();
        private readonly Dictionary<uint, HeroModel> _heroes = new Dictionary<uint, HeroModel>();

        public HeroesModel(GameSettingsProvider config)
        {
            foreach (var hero in config.heroesConfig.heroes) 
                _dataMap[hero.heroType] = hero;
        }

        public HeroModel GetHeroBy(uint id) => _heroes.GetValueOrDefault(id);

        public bool TryGetHeroBy(uint id, out HeroModel model)
        {
            model = GetHeroBy(id);
            return model != null;
        }
        
        public HeroModel Add(uint id, HeroType type)
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
        
        private void OnHealthChanged(uint id)
        {
            if (_heroes[id].currentHealth <= 0)
            {
                _heroes[id].onHealthChanged -= OnHealthChanged;
                _heroes.Remove(id);  
            }
        }
    }
}