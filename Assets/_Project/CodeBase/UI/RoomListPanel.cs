using System;
using System.Collections.Generic;
using Fusion;
using UnityEngine;

namespace CodeBase.UI
{
    public class RoomListPanel : MonoBehaviour
    {
        [SerializeField]
        private RoomListElement _roomPrefab;
        
        [SerializeField]
        private RectTransform _roomsPlaceholder;
        
        private readonly List<RoomListElement> _rooms = new List<RoomListElement>();
        private Action<string> _onJoinPressed;

        public void Initialize(Action<string> onJoinPressed)
        {
            _onJoinPressed = onJoinPressed;
        }

        public void OnRoomListUpdated(List<SessionInfo> roomList)
        {
            var count = _rooms.Count > roomList.Count ? _rooms.Count : roomList.Count;
            for (int i = 0; i < count; i++)
            {
                if (i >= roomList.Count)
                {
                    RemoveRoom(i);
                    continue;
                }
                
                if (i >= _rooms.Count)
                {
                    AddRoom(roomList[i].Name, roomList[i].PlayerCount.ToString());
                    continue;
                }
                
                _rooms[i].Initialize(roomList[i].Name, roomList[i].PlayerCount.ToString(), OnJoinPressed);
            }
        }
        
        private void AddRoom(string roomName, string playerCount)
        {
            var newInstance = Instantiate(_roomPrefab, _roomsPlaceholder);
            newInstance.Initialize(roomName, playerCount, OnJoinPressed);
            _rooms.Add(newInstance);
        }
        
        private void RemoveRoom(int index)
        {
            if (_rooms.Count > index)
            {
                _rooms.Remove(_rooms[index]);
                Destroy(_rooms[index].gameObject);
            }
        }

        private void OnJoinPressed(string roomName) => _onJoinPressed?.Invoke(roomName);
    }
}