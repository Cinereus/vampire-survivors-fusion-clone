using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
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
        private uint _attackerId;
        private float _lifeTimeBorder;
        private TickTimer _lifeTimer;
        private Vector3 _targetDir;
        private AttackService _attackService;

        public void Initialize(uint attackerId, Vector3 targetPos)
        {
            _attackerId = attackerId;
            _targetDir = (targetPos - transform.position).normalized;
        }

        public override void Spawned()
        {
            _attackService = BehaviourInjector.instance.Resolve<AttackService>();
            
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
                var targetDir = _targetDir.normalized;
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