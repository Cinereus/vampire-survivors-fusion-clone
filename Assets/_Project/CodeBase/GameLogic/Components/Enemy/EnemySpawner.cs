using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class EnemySpawner : NetworkBehaviour
    {
        [SerializeField]
        private float _spawnInterval;

        private TickTimer _spawnTimer;
        private EnemySpawnService _spawnService;
        
        public override void Spawned()
        { 
            Debug.Log(gameObject.name + "+ SPAWNED");
            _spawnService = ServiceLocator.instance.Get<EnemySpawnService>();
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _spawnTimer.ExpiredOrNotRunning(Runner))
            {
                _spawnService.SpawnEnemyWave();    
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
            }
        }
    }
}