using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class HeroesInstanceProvider : IService
    {
        private readonly HeroesModel _heroes;
        private readonly Dictionary<Guid, GameObject> _instances = new Dictionary<Guid, GameObject>();

        public HeroesInstanceProvider(HeroesModel heroes)
        {
            _heroes = heroes;
        }

        public List<GameObject> GetAll() => _instances.Values.ToList();
        
        public void AddHero(Guid id, GameObject heroInstance)
        {
            if (_instances.TryGetValue(id, out GameObject _))
            {
                Debug.LogWarning($"{nameof(HeroesInstanceProvider)}: Add failed. Hero instance already exists!");
                return;
            }

            if (!_heroes.TryGetHeroBy(id, out var hero))
            {
                Debug.LogWarning($"{nameof(HeroesInstanceProvider)}: Add failed. Hero model not found!");
                return;
            }
            
            _instances.Add(id, heroInstance);
            hero.onHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(Guid id)
        {
            if (_heroes.TryGetHeroBy(id, out var hero) && hero.currentHealth <= 0)
            {
                hero.onHealthChanged -= OnHealthChanged;
                _instances.Remove(id);   
            }
        }

        public void Dispose()
        {
            foreach (var id in _instances.Keys)
            {
                if (_heroes.TryGetHeroBy(id, out var hero))
                    hero.onHealthChanged -= OnHealthChanged;
            }
            _instances.Clear();
        }
    }
}