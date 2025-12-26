using System;
using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.UI
{
    public class RoomListPanel : MonoBehaviour
    {
        public struct RoomInfo
        {
            public string name;
            public string playerCount;
            public bool isVisited;
        } 
        
        [SerializeField]
        private RoomListElement _roomPrefab;
        
        [SerializeField]
        private RectTransform _roomsPlaceholder;
        
        private readonly List<RoomListElement> _rooms = new List<RoomListElement>();
        private Action<RoomInfo> _onJoinPressed;

        public void Initialize(Action<RoomInfo> onJoinPressed)
        {
            _onJoinPressed = onJoinPressed;
        }

        public void OnRoomListUpdated(List<RoomInfo> roomList)
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
                    AddRoom(roomList[i]);
                    continue;
                }
                
                _rooms[i].Initialize(roomList[i], OnJoinPressed);
            }
        }
        
        private void AddRoom(RoomInfo roomInfo)
        {
            var newInstance = Instantiate(_roomPrefab, _roomsPlaceholder);
            newInstance.Initialize(roomInfo, OnJoinPressed);
            _rooms.Add(newInstance);
        }
        
        private void RemoveRoom(int index)
        {
            if (index < _rooms.Count)
            {
                Destroy(_rooms[index].gameObject);
                _rooms.Remove(_rooms[index]);
            }
        }

        private void OnJoinPressed(RoomInfo roomInfo) => _onJoinPressed?.Invoke(roomInfo);
    }
}