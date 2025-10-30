using System.Collections.Generic;
using System.Linq;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic
{
    public class HeroesInstanceProvider : IService
    {
        private readonly HeroesModel _heroes;
        private readonly NetworkContainer _network;
        private readonly Dictionary<uint, NetworkObject> _instances = new Dictionary<uint, NetworkObject>();

        public HeroesInstanceProvider(HeroesModel heroes, NetworkContainer network)
        {
            _heroes = heroes;
            _network = network;
        }

        public List<NetworkObject> GetAll() => _instances.Values.ToList();
        
        public void AddHero(uint id, NetworkObject heroInstance)
        {
            if (_instances.TryGetValue(id, out NetworkObject _))
            {
                Debug.LogWarning($"{nameof(HeroesInstanceProvider)}: Add failed. Hero instance already exists!");
                return;
            }

            if (!_heroes.TryGetHeroBy(heroInstance.Id.Raw, out var hero))
            {
                Debug.LogWarning($"{nameof(HeroesInstanceProvider)}: Add failed. Hero model not found!");
                return;
            }
            
            _instances.Add(id, heroInstance);
            hero.onHealthChanged += OnHealthChanged;
        }

        private void OnHealthChanged(uint id)
        {
            if (_heroes.TryGetHeroBy(id, out var hero) && hero.currentHealth <= 0)
            {
                hero.onHealthChanged -= OnHealthChanged;
                
                if (_instances.TryGetValue(id, out var heroInstance)) 
                    _network.runner.Despawn(heroInstance);
                
                _instances.Remove(id);
            }
        }

        public void Dispose()
        {
            foreach (var instance in _instances)
            {
                if (_heroes.TryGetHeroBy(instance.Key, out var hero))
                    hero.onHealthChanged -= OnHealthChanged;
                
                _network.runner.Despawn(instance.Value);
            }
            _instances.Clear();
        }
    }
}