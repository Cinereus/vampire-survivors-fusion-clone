using System;
using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase
{
    public class MatchmakingService : IService
    {
        public int roomCount => _rooms.Count;
        public event Action<List<SessionInfo>> onRoomsUpdated;

        private readonly NetworkContainer _network;
        private List<SessionInfo> _rooms = new List<SessionInfo>();

        public MatchmakingService(NetworkContainer network)
        {
            _network = network;
        }

        public void StartLobbySession()
        {
            _network.runner.JoinSessionLobby(SessionLobby.ClientServer);
            _network.callbacks.onSessionListUpdated += OnSessionListUpdated;
        }

        public async void StartGameSession(string roomName, bool isHost, SceneRef scene, Action onCompleted = null,
            Action onFailed = null)
        {
            _network.runner.ProvideInput = true;
            var args = new StartGameArgs
            {
                GameMode = isHost ? GameMode.Host : GameMode.Client,
                SessionName = roomName,
                Scene = scene,
                SceneManager = _network.runner.GetComponent<NetworkSceneManagerDefault>(),
            };

            var result = await _network.runner.StartGame(args);
            if (result.Ok)
            {
                Debug.Log($"{nameof(MatchmakingService)}: Session started successful.");
                onCompleted?.Invoke();
            }
            else
            {
                Debug.LogError($"{nameof(MatchmakingService)}: Failed to start session. Error: {result.ErrorMessage}");
                onFailed?.Invoke();
            }
        }

        public bool CheckRoomExists(string roomName) =>
            _rooms.FirstOrDefault(r => r.Name == roomName) != null;

        private void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> roomList)
        {
            if (runner != _network.runner)
                return;

            onRoomsUpdated?.Invoke(roomList);
            _rooms = roomList;
        }

        public void Dispose()
        {
            _network.callbacks.onSessionListUpdated -= OnSessionListUpdated;
        }

        public void KillSession()
        {
            _network.runner.Shutdown();
        }
    }
}