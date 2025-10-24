using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class HeroChaser : MonoBehaviour
    {
        [SerializeField] private Rigidbody2D _rb;

        [SerializeField] private SpriteRenderer _renderer;

        private readonly List<Transform> _heroesTransforms = new List<Transform>();
        private float _speed;
        private GameFactory _factory;

        public void Setup(float speed, GameFactory factory, List<Transform> heroesTransforms)
        {
            _speed = speed;
            _factory = factory;
            _heroesTransforms.AddRange(heroesTransforms);
            _factory.onHeroCreated += OnHeroCreated;
        }
        
        public void FixedUpdate() => ChaseTarget();

        private void OnDestroy() => _factory.onHeroCreated -= OnHeroCreated;
        
        private void OnHeroCreated(GameObject hero) => _heroesTransforms.Add(hero.transform);

        private void ChaseTarget()
        {
            var chaseDir = GetNearestChaseTargetDir();
            _renderer.flipX = chaseDir.x > 0;
            _rb.MovePosition(_rb.position + chaseDir * (_speed * Time.fixedDeltaTime));
        }

        private Vector2 GetNearestChaseTargetDir()
        {
            var lastSavedDistance = 0f;
            Vector2 result = default;
            foreach (var heroTr in _heroesTransforms)
            {
                Vector2 heroPos = heroTr.position;
                float sqrDistance = (heroPos - _rb.position).sqrMagnitude;
                if (lastSavedDistance == 0 || sqrDistance < lastSavedDistance)
                {
                    lastSavedDistance = sqrDistance;
                    result = (heroPos - _rb.position).normalized;
                }
            }
            return result;
        }
    }
}