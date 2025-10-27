using CodeBase.GameLogic.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class EnemySpawner : NetworkBehaviour
    {
        [SerializeField]
        private float _spawnInterval;

        private Coroutine _spawnRoutine;
        private EnemySpawnService _spawnService;
        private TickTimer _spawnTimer;
        
        public void Setup(EnemySpawnService spawnService)
        {
            _spawnService = spawnService;
        }
        
        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _spawnTimer.ExpiredOrNotRunning(Runner))
            {
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
                _spawnService.SpawnEnemyWave();    
            }
        }

        private void OnDestroy() => StopCoroutine(_spawnRoutine);
    }
}