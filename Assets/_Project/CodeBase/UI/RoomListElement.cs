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

        public void Initialize(string roomName, string playerCount, Action<string> onJoinPressed)
        {
            _roomName.text = roomName;
            _playerCount.text = playerCount;
            _onJoinPressed = onJoinPressed;
        }
        
        public void OnJoinPressed() => _onJoinPressed?.Invoke(_roomName.text);
    }
}