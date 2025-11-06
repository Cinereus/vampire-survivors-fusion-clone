using System.Collections.Generic;
using Fusion;

namespace CodeBase
{
    public static class Exstensions
    {
        public static List<NetworkObject> GetActivePlayerObjects(this NetworkRunner runner)
        {
            var activePlayers = new List<NetworkObject>();
            foreach (var player in runner.ActivePlayers)
            {
                if (runner.TryGetPlayerObject(player, out NetworkObject netObj))
                   activePlayers.Add(netObj);
            }
            return activePlayers;
        }
    }
}