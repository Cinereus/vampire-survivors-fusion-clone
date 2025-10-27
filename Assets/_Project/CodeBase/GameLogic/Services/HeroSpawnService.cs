using CodeBase.Configs.Heroes;
using CodeBase.EntryPoints;
using CodeBase.GameLogic.Components;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class HeroSpawnService : IInitializeService
    {
        private readonly Camera _camera;
        private readonly GameFactory _factory;
        private readonly NetworkContainer _network;
        private readonly HeroesInstanceProvider _instanceProvider;

        public HeroSpawnService(Camera camera, GameFactory factory, NetworkContainer network,
            HeroesInstanceProvider instanceProvider)
        {
            _camera = camera;
            _factory = factory;
            _network = network;
            _instanceProvider = instanceProvider;
        }

        public void Initialize() => _network.callbacks.onPlayerJoined += OnPlayerJoined;

        public void Dispose() => _network.callbacks.onPlayerJoined -= OnPlayerJoined;

        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.IsServer)
            {
                var randOffset = player.RawEncoded % runner.Config.Simulation.PlayerCount * 3;
                var spawnedHeroPos = GetRandomSpawnedHeroPos();
                var spawnPoint = spawnedHeroPos + Vector2.right * randOffset;
                var hero = _factory.CreateHero(HeroType.Knight, spawnPoint);
                if (hero.HasInputAuthority)
                    _camera.GetComponent<CameraFollow>()?.SetTarget(hero.transform);
                
                _instanceProvider.AddHero(hero.Id.Raw, hero);
            }
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