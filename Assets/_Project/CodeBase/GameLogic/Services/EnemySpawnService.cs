using System.Collections.Generic;
using CodeBase.Configs.Enemies;
using CodeBase.GameLogic.Models;
using CodeBase.UI;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Services
{
    public class EnemySpawnService
    {
        private readonly Camera _camera;
        private readonly EnemiesModel _enemies;
        private readonly HeroesInstanceProvider _instanceProvider;
        private readonly GameFactory _factory;

        public EnemySpawnService(UIManager uiManager, GameFactory factory, HeroesInstanceProvider instanceProvider,
            EnemiesModel enemies)
        {
            _camera = uiManager.actualCamera;
            _factory = factory;
            _instanceProvider = instanceProvider;
            _enemies = enemies;
        }

        public void SpawnEnemyWave(List<EnemyData> newEnemies)
        {
            foreach (var enemy in _enemies.GetAvailableEnemyData())
            {
                float probability = 100 * Random.value;
                if (probability < enemy.spawnProbability)
                {
                    var networkObject = _factory.CreateEnemy(enemy.type, GetSpawnPosition());
                    newEnemies.Add(_enemies.GetBy(networkObject.Id.Raw).ToData());
                }
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