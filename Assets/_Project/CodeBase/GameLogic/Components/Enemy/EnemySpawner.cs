using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.GameLogic.Models;
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

        private readonly List<EnemyData> _newEnemiesBuffer = new List<EnemyData>();
        private EnemiesModel _enemies;
        private TickTimer _spawnTimer;
        private EnemySpawnService _spawnService;
        
        public override void Spawned()
        {
            _enemies = BehaviourInjector.instance.Resolve<EnemiesModel>();
            _spawnService = BehaviourInjector.instance.Resolve<EnemySpawnService>();
            _spawnTimer = TickTimer.CreateFromSeconds(Runner, _spawnInterval);
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
        
        [Rpc(RpcSources.StateAuthority, RpcTargets.All, HostMode = RpcHostMode.SourceIsHostPlayer)]
        private void Rpc_RequestActualizeModels(EnemyData[] dataList)
        {
            if (!HasStateAuthority)
                _enemies.Actualize(dataList);
        }
    }
}