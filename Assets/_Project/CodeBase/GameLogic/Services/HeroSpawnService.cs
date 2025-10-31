using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class HeroSpawnService : IService
    {
        private readonly GameFactory _factory;
        private readonly NetworkContainer _network;
        private readonly HeroesInstanceProvider _instanceProvider;

        public HeroSpawnService(GameFactory factory, NetworkContainer network, HeroesInstanceProvider instanceProvider)
        {
            _factory = factory;
            _network = network;
            _instanceProvider = instanceProvider;
        }

        public void Dispose() { }

        public void SpawnHero(PlayerRef player, HeroType heroType)
        {
            var randOffset = Random.Range(-1f, 1f);
            var spawnedHeroPos = GetRandomSpawnedHeroPos();
            var spawnPoint = spawnedHeroPos + Vector2.right * randOffset;
            var hero = _factory.CreateHero(heroType, player, spawnPoint);
            _instanceProvider.AddHero(hero.Id.Raw, hero);
        }
        
        private Vector2 GetRandomSpawnedHeroPos()
        {
            var activePlayers = _instanceProvider.GetAll(); 
            if (activePlayers.Count == 0)
                return Vector2.zero;
            
            return activePlayers[Random.Range(0, activePlayers.Count)].transform.position;
        }
    }
}