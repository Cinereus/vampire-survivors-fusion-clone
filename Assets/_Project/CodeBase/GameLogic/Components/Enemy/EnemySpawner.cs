using System.Collections;
using CodeBase.GameLogic.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class EnemySpawner : MonoBehaviour
    {
        [SerializeField]
        private float _spawnInterval;

        private Coroutine _spawnRoutine;
        private EnemySpawnService _spawnService;

        public void Setup(EnemySpawnService spawnService)
        {
            _spawnService = spawnService;
            _spawnRoutine = StartCoroutine(SpawnWithInterval());
        }

        private void OnDestroy() => StopCoroutine(_spawnRoutine);

        private IEnumerator SpawnWithInterval()
        {
            var waitForSeconds = new WaitForSeconds(_spawnInterval);
            while (true)
            {
                yield return waitForSeconds;
                _spawnService.SpawnEnemyWave();
            }
        }
    }
}