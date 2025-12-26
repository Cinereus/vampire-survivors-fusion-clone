using System;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class RoomListElement : MonoBehaviour
    {
        [SerializeField]
        private TextMeshProUGUI _roomName;
        
        [SerializeField]
        private TextMeshProUGUI _playerCount;

        private Action<RoomListPanel.RoomInfo> _onJoinPressed;
        private RoomListPanel.RoomInfo _roomInfo;

        public void Initialize(RoomListPanel.RoomInfo roomInfo, Action<RoomListPanel.RoomInfo> onJoinPressed)
        {
            _roomInfo = roomInfo;
            _roomName.text = roomInfo.name;
            _playerCount.text = roomInfo.playerCount;
            _roomName.color = roomInfo.isVisited ? Color.red : Color.white;
            _onJoinPressed = onJoinPressed;
        }
        
        public void OnJoinPressed() => _onJoinPressed?.Invoke(_roomInfo);
    }
}