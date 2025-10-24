using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class EnemySpawnService : IService
    {
        private readonly Camera _camera;
        private readonly GameFactory _factory;
        private readonly List<EnemyData> _enemies;
        private readonly List<Transform> _heroesTransforms;

        public EnemySpawnService(Camera camera, GameFactory factory, EnemiesModel enemies)
        {
            _camera = camera;
            _factory = factory;
            _enemies = enemies.GetAvailableEnemyData();
            _factory.onHeroCreated += OnHeroCreated;
            _heroesTransforms = _factory.heroActiveInstances;
        }
        
        public void Dispose()
        {
            _factory.onHeroCreated -= OnHeroCreated;
        }
        
        public void SpawnEnemyWave()
        {
            foreach (var enemy in _enemies)
            {
                const int FULL_PROBABILITY = 100;
                if (FULL_PROBABILITY * Random.value < enemy.spawnProbability) 
                    _factory.CreateEnemy(enemy.type, GetSpawnPosition());
            }
        }

        private void OnHeroCreated(GameObject hero) => _heroesTransforms.Add(hero.transform);

        private Vector3 GetSpawnPosition()
        {
            Vector3 min = _camera.ViewportToWorldPoint(new Vector3(0, 0, _camera.nearClipPlane));
            Vector3 max = _camera.ViewportToWorldPoint(new Vector3(1, 1, _camera.nearClipPlane));
            var outsideOffset = 4f;
            float randHorizontalSide = Random.Range(min.x - outsideOffset, max.x + outsideOffset);
            float randVerticalSide = Random.Range(min.y - outsideOffset, max.y + outsideOffset);
            var result = new Vector3(randHorizontalSide, randVerticalSide, 0);
            if (CheckPositionFarFromHeroes(result))
                return result;
            
            return GetSpawnPosition();
        }

        private bool CheckPositionFarFromHeroes(Vector2 pos)
        {
            foreach (var heroPos in _heroesTransforms)
            {
                const float EPSILON = 0.01f;
                float distance = (pos - (Vector2) heroPos.position).sqrMagnitude;
                if (distance <= EPSILON)
                    return false;
            }
            return true;
        }
    }
}