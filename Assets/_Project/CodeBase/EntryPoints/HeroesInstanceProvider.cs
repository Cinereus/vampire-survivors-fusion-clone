using System.Collections.Generic;
using System.Linq;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class HeroesInstanceProvider : IService
    {
        private readonly HeroesModel _heroes;
        private readonly NetworkContainer _container;
        private readonly Dictionary<uint, NetworkObject> _instances = new Dictionary<uint, NetworkObject>();

        public HeroesInstanceProvider(HeroesModel heroes, NetworkContainer container)
        {
            _heroes = heroes;
            _container = container;
        }

        public List<NetworkObject> GetAll() => _instances.Values.ToList();
        
        public void AddHero(uint id, NetworkObject heroInstance)
        {
            if (!_container.runner.IsServer)
            {
                Debug.Log($"{nameof(HeroesInstanceProvider)}: Add failed. User not the host.");
                return;
            }
            
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