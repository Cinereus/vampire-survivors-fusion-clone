using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.Infrastructure.Services.Configs;
using CodeBase.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Services
{
    public class EnemySpawnService
    {
        private readonly Camera _camera;
        private readonly GameFactory _factory;
        private readonly HeroesInstanceProvider _instanceProvider;
        private readonly List<EnemyData> _availableEnemies = new List<EnemyData>();

        public EnemySpawnService(UIManager uiManager, GameFactory factory, HeroesInstanceProvider instanceProvider,
            IConfigProvider configProvider)
        {
            _factory = factory;
            _camera = uiManager.actualCamera;
            _instanceProvider = instanceProvider;
            _availableEnemies.AddRange(configProvider.GetConfig<EnemiesConfig>().enemies);
        }

        public void SpawnEnemyWave()
        {
            foreach (var enemy in _availableEnemies)
            {
                float probability = 100 * Random.value;
                if (probability < enemy.spawnProbability) 
                    _factory.CreateEnemy(enemy.enemyType, GetSpawnPosition());
            }
        }

        private Vector2 GetSpawnPosition()
        {
            float screenHeight = _camera.orthographicSize * 2f;
            float screenWidth = screenHeight * _camera.aspect;
            float outsideOffset = Random.Range(0.5f, 2f);
            float horizontalBorder = screenWidth / 2f + outsideOffset;
            float verticalBorder = screenHeight / 2f + outsideOffset;
            Vector2 spawnPos = Vector2.zero;
            
            if (GetRandom())
            {
                spawnPos.x = GetRandom() ? horizontalBorder : -horizontalBorder;
                spawnPos.y = Random.Range(-verticalBorder, verticalBorder);
            }
            else
            {
                spawnPos.x = Random.Range(-horizontalBorder, horizontalBorder);
                spawnPos.y = GetRandom() ? verticalBorder : -verticalBorder;
            }
         
            var heroes = _instanceProvider.GetAll();
            spawnPos += heroes.Count == 0 ? Vector2.zero : heroes[Random.Range(0, heroes.Count)].transform.position;
            
            return CheckPositionFarFromHeroes(spawnPos) ? spawnPos : GetSpawnPosition();
        }

        private bool GetRandom() => Random.Range(0, 2) > 0;

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