using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.EntryPoints;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class EnemySpawnService : IService
    {
        private readonly Camera _camera;
        private readonly List<EnemyData> _enemies;
        private readonly HeroesInstanceProvider _instanceProvider;
        private readonly GameFactory _factory;

        public EnemySpawnService(Camera camera, GameFactory factory, HeroesInstanceProvider instanceProvider,
            EnemiesModel enemies)
        {
            _camera = camera;
            _factory = factory;
            _instanceProvider = instanceProvider;
            _enemies = enemies.GetAvailableEnemyData();
        }
        
        public void Dispose() { }
        
        public void SpawnEnemyWave()
        {
            foreach (var enemy in _enemies)
            {
                const int FULL_PROBABILITY = 100;
                if (FULL_PROBABILITY * Random.value < enemy.spawnProbability) 
                    _factory.CreateEnemy(enemy.type, GetSpawnPosition());
            }
        }
        
        private Vector2 GetSpawnPosition()
        {
            float height = _camera.orthographicSize * 2f;
            float width = height * _camera.aspect;
            var outsideOffset = 4f;
            float randHorizontalSide = Random.Range(-width / 2f - outsideOffset, width / 2f + outsideOffset);
            float randVerticalSide = Random.Range(-height / 2f - outsideOffset, height / 2f + outsideOffset);
            var camPos = _camera.transform.position;
            var result = new Vector2(camPos.x + randHorizontalSide, camPos.y + randVerticalSide);
            if (CheckPositionFarFromHeroes(result))
                return result;
            
            return GetSpawnPosition();
        }

        private bool CheckPositionFarFromHeroes(Vector2 pos)
        {
            foreach (var heroPos in _instanceProvider.GetAll())
            {
                const float EPSILON = 0.01f;
                float distance = (pos - (Vector2) heroPos.transform.position).sqrMagnitude;
                if (distance <= EPSILON)
                    return false;
            }
            return true;
        }
    }
}