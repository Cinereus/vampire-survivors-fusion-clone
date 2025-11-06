using System.Collections.Generic;
using System.Linq;
using CodeBase.Infrastructure.Services;
using Fusion;

namespace CodeBase.GameLogic
{
    public class HeroesInstanceProvider : IService
    {
        private readonly List<NetworkObject> _instances = new List<NetworkObject>();
        
        public List<NetworkObject> GetAll() => _instances;

        public void Clear() => _instances.Clear();
        
        public void ActualizeInstances(List<NetworkObject> activePlayers)
        {
            _instances.Clear();
            _instances.AddRange(activePlayers);
        }
        
        public void Remove(PlayerRef player)
        {
            var netObject = _instances.FirstOrDefault(i => i.InputAuthority == player);
            if (!netObject)
                return;
            
            _instances.Remove(netObject);
        }
        
        public void Dispose()
        {
            _instances.Clear();
        }
    }
}