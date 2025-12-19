using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.UI;
using Fusion;
using UnityEngine;

namespace CodeBase
{
    public class MatchmakingService : IDisposable
    {
        public int roomCount => _rooms.Count;
        public event Action<List<SessionInfo>> onRoomsUpdated;

        private readonly NetworkProvider _network;
        private readonly LoadSceneService _sceneService;
        private readonly GameAnalytics _analytics;
        private readonly UIManager _uiManager;
        private readonly List<SessionInfo> _rooms = new List<SessionInfo>();

        public MatchmakingService(NetworkProvider network, LoadSceneService sceneService, UIManager uiManager,
            GameAnalytics analytics)
        {
            _network = network;
            _sceneService = sceneService;
            _analytics = analytics;
            _uiManager = uiManager;
        }

        public void Initialize()
        {
            _network.callbacks.onSessionListUpdated += OnSessionListUpdated;
        }

        public async void StartLobbySession(Action onCompleted = null, Action onFailed = null)
        {
            _uiManager.ShowLoadingScreen();
            var result = await _network.runner.JoinSessionLobby(SessionLobby.ClientServer);
            _uiManager.HideLoadingScreen();
            
            if (result.Ok)
            {
                Debug.Log("Lobby joined");
                onCompleted?.Invoke();
            }
            else
            {
                Debug.LogError($"Failed to join session lobby: {result.ErrorMessage}");
                onFailed?.Invoke();
            }
        }

        public async void MigrateGameSession(HostMigrationToken token, Action<NetworkRunner> onHostMigrationResume,
            Action onCompleted = null, Action onFailed = null)
        {
            _uiManager.ShowLoadingScreen();
            await _network.ClearCurrentRunner(ShutdownReason.HostMigration);
            
            _network.runner.ProvideInput = true;
            var args = new StartGameArgs
            {
                Scene = SceneRef.FromIndex(_sceneService.GetActiveScene().buildIndex),
                HostMigrationToken = token,
                HostMigrationResume = onHostMigrationResume,
                SceneManager = _network.runner.GetComponent<NetworkSceneManagerDefault>(),
            };

            await StartGameSession(args, onCompleted, onFailed);
            _uiManager.HideLoadingScreen();
        }

        public async void StartGameSession(string roomName, bool isHost, string scene, Action onCompleted = null,
            Action onFailed = null)
        {
            _uiManager.ShowLoadingScreen();
            await _sceneService.LoadSceneAsync(scene, needShowLoadingScreen: false);
            
            _network.runner.ProvideInput = true;
            var args = new StartGameArgs
            {
                GameMode = isHost ? GameMode.Host : GameMode.Client,
                SessionName = roomName,
                Scene = SceneRef.FromIndex(_sceneService.GetActiveScene().buildIndex),
                SceneManager = _network.runner.GetComponent<NetworkSceneManagerDefault>(),
            };
            await StartGameSession(args, onCompleted, onFailed);
            _uiManager.HideLoadingScreen();
        }

        public bool CheckRoomExists(string roomName) => _rooms.FirstOrDefault(r => r.Name == roomName) != null;

        public void Dispose()
        {
            _network.callbacks.onSessionListUpdated -= OnSessionListUpdated;
            _rooms.Clear();
        }

        public async Task KillSession()
        {
            var sessionName = _network.runner.SessionInfo.Name;
            await _network.runner.Shutdown();
            _analytics.SendGameSessionFinished(sessionName);
            _rooms.Clear();
        }

        private async Task StartGameSession(StartGameArgs args, Action onCompleted = null, Action onFailed = null)
        {
            var isMigration = args.HostMigrationToken != null;
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
            _analytics.SendGameSessionStarted(args.SessionName, result.Ok, isMigration);
        }
        
        private void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> roomList)
        {
            if (runner != _network.runner)
                return;
            
            onRoomsUpdated?.Invoke(roomList);
            _rooms.Clear();
            _rooms.AddRange(roomList);
        }
    }
}