using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Configs.Heroes;
using Fusion;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class MainMenuPanel : MonoBehaviour
    {
        [SerializeField] 
        private TMP_Dropdown _heroTypeDropdown;

        [SerializeField] 
        private RoomListPanel _roomListPanel;

        [SerializeField] 
        private TMP_InputField _hostInputField;

        public event Action<string> onStartAsHost;
        public event Action<string> onJoinRoomFromList;
        public event Action<HeroType> onHeroSelected;

        private List<HeroData> _heroes;

        public void Setup(List<HeroData> heroes)
        {
            _heroes = heroes;
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

        private void SelectHero(int index)
        {
            if (index > _heroTypeDropdown.options.Count)
                return;

            onHeroSelected?.Invoke(_heroes[index].heroType);
        }

        private void OnJoinRoomFromListPressed(string roomName) => onJoinRoomFromList?.Invoke(roomName);

        private void InitializeHeroDropdown()
        {
            var options = new List<TMP_Dropdown.OptionData>();
            foreach (var heroData in _heroes)
                options.Add(new TMP_Dropdown.OptionData(heroData.heroType.ToString()));

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