using CodeBase.Infrastructure;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Enemy
{
    public class HeroChaser : NetworkBehaviour
    {
        [SerializeField] 
        private Rigidbody2D _rb;

        [SerializeField] 
        private SpriteRenderer _renderer;
        
        [Networked]
        private Vector2 moveDir { get; set; }
        
        private HeroesInstanceProvider _instanceProvider;
        private float _speed;
        
        public void Initialize(float speed)
        { 
            _speed = speed;
        }

        public override void Spawned()
        {
            _instanceProvider = BehaviourInjector.instance.Resolve<HeroesInstanceProvider>();
        }

        public override void FixedUpdateNetwork()
        {
            ChaseTarget();
        }

        public override void Render()
        {
            _renderer.flipX = moveDir.x > 0;
        }

        private void ChaseTarget()
        {
            if (HasStateAuthority)
            {
                moveDir = GetNearestChaseTargetDir();
                _rb.MovePosition(_rb.position + moveDir * (_speed * Runner.DeltaTime));    
            }
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