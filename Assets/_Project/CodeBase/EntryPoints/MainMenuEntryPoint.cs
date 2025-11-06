using System.Collections.Generic;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic;
using CodeBase.Infrastructure.Services;
using CodeBase.UI;
using Fusion;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private MainMenuPanel _mainMenuPanel;

        [SerializeField] 
        private GameObject _loadingScreen;

        private PlayerData _playerData;
        private MatchmakingService _matchmakingService;
        private LoadSceneService _sceneService;
        
        private void Awake()
        {
            var services = ServiceLocator.instance;
            SetupDependencies(services);
            InitializeMainMenuPanel(services);
            InitializeMatchmaking();
        }

        private void InitializeMainMenuPanel(ServiceLocator services)
        {
            _mainMenuPanel.Setup(services.Get<GameSettingsProvider>().heroesConfig.heroes);
            _mainMenuPanel.onStartAsHost += OnStartAsHost;
            _mainMenuPanel.onJoinRoomFromList += OnJoinRoomFromList;
            _mainMenuPanel.onHeroSelected += OnHeroSelected;
        }

        private void InitializeMatchmaking()
        {
            _matchmakingService.StartLobbySession(onCompleted: () => _loadingScreen.SetActive(false));
            _matchmakingService.onRoomsUpdated += OnRoomListUpdated;
        }

        private void OnDestroy()
        {
            _mainMenuPanel.onStartAsHost -= OnStartAsHost;
            _mainMenuPanel.onJoinRoomFromList -= OnJoinRoomFromList;
            _mainMenuPanel.onHeroSelected -= OnHeroSelected;
            _matchmakingService.onRoomsUpdated -= OnRoomListUpdated;
        }

        private void OnRoomListUpdated(List<SessionInfo> roomList) =>
            _mainMenuPanel.OnRoomListUpdated(roomList, _playerData.visitedRooms.Contains);

        private void OnHeroSelected(HeroType heroType) => _playerData.chosenHero = heroType;
        
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

        private void SetupDependencies(ServiceLocator services)
        {
            _playerData = services.Get<PlayerData>();
            _matchmakingService = services.Get<MatchmakingService>();
            _sceneService = services.Get<LoadSceneService>();
        }
    }
}