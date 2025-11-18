using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
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
            if (HasStateAuthority)
            {
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
                _spawnService = BehaviourInjector.instance.Resolve<EnemySpawnService>();    
            }
        }

        public override void FixedUpdateNetwork()
        { 
            SpawnEnemies();
        }

        private void SpawnEnemies()
        {
            if (HasStateAuthority && _spawnTimer.ExpiredOrNotRunning(Runner))
            {
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
                _spawnService.SpawnEnemyWave();
            }
        }
    }
}