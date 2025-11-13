using System;
using System.Collections.Generic;
using System.Linq;
using Fusion;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class MainMenuScreen : BaseUIEntity
    {
        [SerializeField] 
        private TMP_Dropdown _heroTypeDropdown;

        [SerializeField] 
        private RoomListPanel _roomListPanel;

        [SerializeField] 
        private TMP_InputField _hostInputField;

        private readonly List<string> _heroNames = new List<string>();
        
        public event Action<string> onStartAsHost;
        public event Action<string> onJoinRoomFromList;
        public event Action<int> onHeroSelected;
        
        public void Initialize(List<string> heroNames)
        {
            _heroNames.AddRange(heroNames);
            InitializeHeroDropdown();
            InitializeRoomListPanel();
        }

        public void OnStartAsHostPressed() => onStartAsHost?.Invoke(_hostInputField.text);

        public void OnRoomListUpdated(List<SessionInfo> roomList, Predicate<string> checkVisited)
        {
            var rooms = roomList.Select(r =>
                new RoomListPanel.RoomInfo
                {
                    name = r.Name,
                    playerCount = r.PlayerCount.ToString(),
                    isVisited = checkVisited.Invoke(r.Name)
                }
            ).ToList();

            _roomListPanel.OnRoomListUpdated(rooms);
        }

        public override void Dispose()
        {
            _heroNames.Clear();
            base.Dispose();
        }

        private void SelectHero(int index)
        {
            if (index > _heroTypeDropdown.options.Count)
                return;

            onHeroSelected?.Invoke(index);
        }

        private void OnJoinRoomFromListPressed(string roomName) => onJoinRoomFromList?.Invoke(roomName);

        private void InitializeHeroDropdown()
        {
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var heroName in _heroNames)
                options.Add(new TMP_Dropdown.OptionData(heroName));

            _heroTypeDropdown.options = options;
            SelectHero(_heroTypeDropdown.value);
            _heroTypeDropdown.onValueChanged.AddListener(SelectHero);
        }

        private void InitializeRoomListPanel()
        {
            _roomListPanel.Initialize(OnJoinRoomFromListPressed);
        }
    }
}