using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class AttackProjectile : NetworkBehaviour
    {
        [SerializeField]
        private float _lifeTime;
        
        [SerializeField]
        private float _moveSpeed;
        
        [SerializeField]
        private CollisionTracker _hurtBox;
        
        private uint _id;
        private float _lifeTimeBorder;
        private Vector3 _targetPos;
        private AttackService _attackService;
        private TickTimer _lifeTimer;
        private uint _attackerId;

        public void Setup(uint attackerId, Vector3 targetPos)
        {
            _attackerId = attackerId;
            _targetPos = targetPos;
        }

        public override void Spawned()
        {
            _attackService = ServiceLocator.instance.Get<AttackService>();
            
            if (HasStateAuthority)
            {
                _lifeTimer = TickTimer.CreateFromSeconds(Runner, _lifeTime);
                _hurtBox.onTriggerEnter += OnVictimEntered;    
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _hurtBox.onTriggerEnter -= OnVictimEntered;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _lifeTimer.Expired(Runner))
            {
                Runner.Despawn(Object);
            }
            else
            {
                var targetDir = (_targetPos - transform.position).normalized;
                transform.position += targetDir * (_moveSpeed * Runner.DeltaTime);    
            }
        }

        private void OnVictimEntered(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (victimId.HasValue)
            {
                _attackService.MakeAttack(_attackerId, victimId.Value);
                Runner.Despawn(Object);
            }
        }
    }
}