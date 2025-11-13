using System;
using CodeBase.GameLogic;
using CodeBase.Infrastructure;
using CodeBase.UI;
using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class GameEntryPoint : IInitializable, IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly GameFactory _factory;
        private readonly NetworkProvider _network;
        private readonly MatchmakingService _matchmakingService;
        private readonly Camera _mainCamera;
        private readonly IObjectResolver _resolver;
        private readonly LoadSceneService _sceneService;

        public GameEntryPoint(GameFactory factory, NetworkProvider network, UIManager uiManager,
            LoadSceneService sceneService, MatchmakingService matchmakingService, Camera mainCamera,
            IObjectResolver resolver)
        {
            _factory = factory;
            _network = network;
            _uiManager = uiManager;
            _sceneService = sceneService;
            _matchmakingService = matchmakingService;
            _mainCamera = mainCamera;
            _resolver = resolver;
        }
        
        public void Initialize()
        {
            _uiManager.SetupActualCamera(_mainCamera);
            BehaviourInjector.instance.SetupResolver(_resolver);
            _network.callbacks.onSceneLoadDone += OnSceneLoadDone;
            _network.callbacks.onShutdown += OnShutdown;
            _network.callbacks.onPlayerJoined += OnPlayerJoined;
        }
        
        public void Dispose()
        {
            _network.callbacks.onSceneLoadDone -= OnSceneLoadDone;
            _network.callbacks.onShutdown -= OnShutdown;
            _network.callbacks.onPlayerJoined -= OnPlayerJoined;
            _uiManager.Hide<GameOverScreen>();
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