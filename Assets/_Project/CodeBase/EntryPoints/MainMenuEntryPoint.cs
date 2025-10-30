using System.Collections.Generic;
using CodeBase.GameLogic;
using CodeBase.Infrastructure.Services;
using CodeBase.UI;
using TMPro;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private TMP_Dropdown _heroTypeDropdown;
        
        [SerializeField]
        private RoomListPanel _roomListPanel;
        
        [SerializeField]
        private TMP_InputField _hostInputField;

        private PlayerData _playerData; 
        private GameSettingsProvider _gameSettings;
        private MatchmakingService _matchmakingService;
        private LoadSceneService _sceneService;

        public void OnStartAsHostPressed()
        {
            if (string.IsNullOrEmpty(_hostInputField.text))
            {
                var roomName = $"room#{_matchmakingService.roomCount + 1}";
                StartGame(roomName, true);
                return;
            }

            if (!_matchmakingService.CheckRoomExists(_hostInputField.text)) 
                StartGame(_hostInputField.text, true);
        }

        private void Awake()
        {
            var services = ServiceLocator.instance; 
            RegisterServices(services);
            SetupDependencies(services);
            InitializeHeroDropdown();
            InitializeRoomListPanel();
            _matchmakingService.StartLobbySession();
        }

        private void OnDestroy()
        {
            _matchmakingService.onRoomsUpdated -= _roomListPanel.OnRoomListUpdated;
        }

        private void RegisterServices(ServiceLocator services)
        {
            services.Register(new PlayerData(), ServiceContext.Game);
            services.Register(new MatchmakingService(services.Get<NetworkContainer>()), ServiceContext.Game);
        }

        private void SetupDependencies(ServiceLocator services)
        {
            _playerData = services.Get<PlayerData>();
            _gameSettings = services.Get<GameSettingsProvider>();
            _matchmakingService = services.Get<MatchmakingService>();
            _sceneService = services.Get<LoadSceneService>();
        }
        
        private void InitializeHeroDropdown()
        {
            var heroDataList = _gameSettings.heroesConfig.heroes;
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var heroData in heroDataList)
                options.Add(new TMP_Dropdown.OptionData(heroData.heroType.ToString()));
            
            _heroTypeDropdown.options = options;
            SelectHero(_heroTypeDropdown.value);
            _heroTypeDropdown.onValueChanged.AddListener(SelectHero);
        }
        
        private void InitializeRoomListPanel()
        {
            _roomListPanel.Initialize(OnJoinRoomFromListPressed);
            _matchmakingService.onRoomsUpdated += _roomListPanel.OnRoomListUpdated;
        }
        
        private void SelectHero(int index)
        {
            if (index > _heroTypeDropdown.options.Count)
                return;
            
            _playerData.chosenHero = _gameSettings.heroesConfig.heroes[index].heroType;
        }
        
        private void OnJoinRoomFromListPressed(string roomName) => 
            StartGame(roomName, false);

        private void StartGame(string roomName, bool isHost)
        {
            _playerData.roomName = roomName;
            _playerData.isHost = isHost;
            _sceneService.LoadScene(SceneNames.GAME);
        }
    }
}
