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

        private Action<string> _onJoinPressed;

        public void Initialize(RoomListPanel.RoomInfo roomInfo, Action<string> onJoinPressed)
        {
            _roomName.text = roomInfo.name;
            _playerCount.text = roomInfo.playerCount;
            _roomName.color = roomInfo.isVisited ? Color.red : Color.white;
            _onJoinPressed = roomInfo.isVisited ? null : onJoinPressed;
        }
        
        public void OnJoinPressed() => _onJoinPressed?.Invoke(_roomName.text);
    }
}