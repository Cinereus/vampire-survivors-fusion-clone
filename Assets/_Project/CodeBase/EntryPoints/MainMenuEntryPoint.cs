using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Services.SaveLoad;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.UI;
using Fusion;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : IAsyncStartable, IDisposable
    {
        private readonly UIManager _uiManager;
        private readonly PlayerData _playerData;
        private readonly AssetProvider _assetProvider;
        private readonly LoadSceneService _sceneService;
        private readonly MatchmakingService _matchmakingService;
        private readonly ISaveLoadService _saveLoad;
        private readonly IObjectResolver _resolver;
        private readonly Camera _mainCamera;

        private MainMenuScreen _mainMenuScreen;

        public MainMenuEntryPoint(UIManager uiManager, AssetProvider assetProvider, PlayerData playerData,
            LoadSceneService sceneService, MatchmakingService matchmakingService, Camera mainCamera,
            ISaveLoadService saveLoad,  IObjectResolver resolver)
        {
            _assetProvider = assetProvider;
            _playerData = playerData;
            _matchmakingService = matchmakingService;
            _mainCamera = mainCamera;
            _sceneService = sceneService;
            _uiManager = uiManager;
            _saveLoad = saveLoad;
            _resolver = resolver;
        }
        
        public Awaitable StartAsync(CancellationToken token)
        {
            BehaviourInjector.instance.SetupResolver(_resolver);
            InitializeMainMenuPanel();
            InitializeMatchmaking();
            return Awaitable.EndOfFrameAsync(token);
        }

        public void Dispose()
        {
            _saveLoad.Save();
            
            if (_mainMenuScreen != null)
            {
                _mainMenuScreen.onStartAsHost -= OnStartAsHost;
                _mainMenuScreen.onJoinRoomFromList -= OnJoinRoomFromList;
                _mainMenuScreen.onHeroSelected -= OnHeroSelected;
                _uiManager.Hide<MainMenuScreen>();
            }

            if (_matchmakingService != null)
            {
                _matchmakingService.onRoomsUpdated -= OnRoomListUpdated;
            }
        }

        private void InitializeMainMenuPanel()
        {
            _uiManager.SetupActualCamera(_mainCamera);
            _mainMenuScreen = _uiManager.Show<MainMenuScreen>();

            var heroNames = _assetProvider.GetConfig<HeroesConfig>().heroes
                .Select(h => h.heroType.ToString()).ToList();

            _mainMenuScreen.Initialize((int) _playerData.chosenHero, heroNames);

            _mainMenuScreen.onStartAsHost += OnStartAsHost;
            _mainMenuScreen.onJoinRoomFromList += OnJoinRoomFromList;
            _mainMenuScreen.onHeroSelected += OnHeroSelected;
        }

        private void InitializeMatchmaking()
        {
            _matchmakingService.StartLobbySession();
            _matchmakingService.onRoomsUpdated += OnRoomListUpdated;
        }

        private void OnRoomListUpdated(List<SessionInfo> roomList)
        {
            _mainMenuScreen.OnRoomListUpdated(roomList, _playerData.visitedRooms.Contains);
        }

        private void OnHeroSelected(int hero) => _playerData.chosenHero = (HeroType) hero;

        private void OnJoinRoomFromList(string roomName) => StartGame(roomName, false);

        private void OnStartAsHost(string roomName)
        {
            if (string.IsNullOrEmpty(roomName))
            {
                roomName = $"room#{_matchmakingService.roomCount + 1}";
                StartGame(roomName, true);
                return;
            }

            if (!_matchmakingService.CheckRoomExists(roomName))
                StartGame(roomName, true);
        }

        private void StartGame(string roomName, bool isHost)
        {
            _playerData.visitedRooms.Add(roomName);
            _matchmakingService.StartGameSession(roomName, isHost, SceneNames.GAME,
                onFailed: () => _sceneService.LoadScene(SceneNames.MAIN_MENU));
        }
    }
}