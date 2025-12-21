using System;
using System.Threading;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI;
using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class GameEntryPoint : IAsyncStartable, IDisposable
    {
        private readonly Heroes _heroes;
        private readonly Enemies _enemies;
        private readonly Camera _mainCamera;
        private readonly UIManager _uiManager;
        private readonly GameFactory _factory;
        private readonly NetworkProvider _network;
        private readonly IObjectResolver _resolver;
        private readonly AssetProvider _assetProvider;
        private readonly LoadSceneService _sceneService;
        private readonly MatchmakingService _matchmakingService;

        public GameEntryPoint(GameFactory factory, NetworkProvider network, UIManager uiManager,
            LoadSceneService sceneService, MatchmakingService matchmakingService, Camera mainCamera,
            IObjectResolver resolver, AssetProvider assetProvider, Heroes heroes, Enemies enemies)
        {
            _heroes = heroes;
            _enemies = enemies;
            _factory = factory;
            _network = network;
            _resolver = resolver;
            _uiManager = uiManager;
            _mainCamera = mainCamera;
            _sceneService = sceneService;
            _assetProvider = assetProvider;
            _matchmakingService = matchmakingService;
        }
        
        public async Awaitable StartAsync(CancellationToken _)
        {
            await _assetProvider.PrepareGameAssetGroup();
            
            _uiManager.SetupActualCamera(_mainCamera);
            BehaviourInjector.instance.SetupResolver(_resolver);
            _heroes.Initialize(_assetProvider.GetConfig<HeroesConfig>().heroes, data => (uint) data.heroType);
            _enemies.Initialize(_assetProvider.GetConfig<EnemiesConfig>().enemies, data => (uint) data.enemyType);
            
            _network.callbacks.onSceneLoadDone += OnSceneLoadDone;
            _network.callbacks.onPlayerJoined += OnPlayerJoined;
            _network.callbacks.onShutdown += OnShutdown;
        }
        
        public void Dispose()
        {
            _network.callbacks.onSceneLoadDone -= OnSceneLoadDone;
            _network.callbacks.onPlayerJoined -= OnPlayerJoined;
            _network.callbacks.onShutdown -= OnShutdown;
            _uiManager.Hide<GameOverScreen>();
            _assetProvider.ReleaseGameAssetGroup();
        }
        
        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer == player)
                _factory.CreateHeroSpawner();
        }

        private void OnSceneLoadDone(NetworkRunner runner)
        {
            _factory.CreateEnemySpawner();
        }

        private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            _uiManager.Show<GameOverScreen>().Initialize(_matchmakingService, _sceneService);
        }
    }
}