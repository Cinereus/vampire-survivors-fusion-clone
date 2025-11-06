using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.GameLogic.Models;
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

        private readonly List<EnemyData> _newEnemiesBuffer = new List<EnemyData>();
        private EnemySpawnService _spawnService;
        private EnemiesModel _enemies;
        private TickTimer _spawnTimer;

        public override void Spawned()
        {
            _enemies = ServiceLocator.instance.Get<EnemiesModel>();
            _spawnService = ServiceLocator.instance.Get<EnemySpawnService>();
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _spawnTimer.ExpiredOrNotRunning(Runner))
            {
                _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
                _spawnService.SpawnEnemyWave(_newEnemiesBuffer);
                
                Rpc_RequestActualizeModels(_newEnemiesBuffer.ToArray());
                _newEnemiesBuffer.Clear();
            }
        }
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsServer)]
        private void Rpc_RequestActualizeModels(EnemyData[] dataList)
        {
            if (!HasStateAuthority)
                _enemies.Actualize(dataList);
        }
    }
}