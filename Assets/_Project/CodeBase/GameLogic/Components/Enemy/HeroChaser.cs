using CodeBase.EntryPoints;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class HeroChaser : MonoBehaviour
    {
        [SerializeField] 
        private Rigidbody2D _rb;

        [SerializeField] 
        private SpriteRenderer _renderer;
        
        private HeroesInstanceProvider _instanceProvider;
        private float _speed;

        public void Setup(float speed, HeroesInstanceProvider instanceProvider)
        {
            _speed = speed;
            _instanceProvider = instanceProvider;
        }
        
        public void FixedUpdate() => ChaseTarget();

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
            foreach (var heroTr in _instanceProvider.GetAll())
            {
                Vector2 heroPos = heroTr.transform.position;
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