using System;
using System.Collections.Generic;
using Fusion;
using Fusion.Sockets;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Network
{
    public class NetworkRunnerCallbacks : MonoBehaviour, INetworkRunnerCallbacks
    {
       
        public event Action<NetworkRunner, List<SessionInfo>> onSessionListUpdated;
        public event Action<NetworkRunner, NetworkInput> onInput;
        public event Action<NetworkRunner, PlayerRef> onPlayerJoined;
        public event Action<NetworkRunner, PlayerRef> onPlayerLeft;
        public event Action<NetworkRunner, HostMigrationToken> onHostMigration;
        public event Action<NetworkRunner, ShutdownReason> onShutdown;
        public event Action<NetworkRunner> onSceneLoadDone;

        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) => onPlayerJoined?.Invoke(runner, player);
        
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) => onPlayerLeft?.Invoke(runner, player);
        
        public void OnInput(NetworkRunner runner, NetworkInput input) => onInput?.Invoke(runner, input);

        public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) =>
            onSessionListUpdated?.Invoke(runner, sessionList);

        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) =>
            onHostMigration?.Invoke(runner, hostMigrationToken);
        
        public void OnSceneLoadDone(NetworkRunner runner) => onSceneLoadDone?.Invoke(runner);

        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) =>
            onShutdown?.Invoke(runner, shutdownReason);
        
        public void OnConnectedToServer(NetworkRunner runner)
        {
        }

        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason)
        {
        }
        
        public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player)
        {
        }

        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request,
            byte[] token)
        {
        }

        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
        {
        }

        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
        {
        }

        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key,
            ArraySegment<byte> data)
        {
        }

        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress)
        {
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
        {
        }

        public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
        {
        }

        public void OnSceneLoadStart(NetworkRunner runner)
        {
        }
    }
}