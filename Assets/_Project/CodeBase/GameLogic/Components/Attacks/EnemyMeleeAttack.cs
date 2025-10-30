using System.Collections.Generic;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class EnemyMeleeAttack : NetworkBehaviour
    {
        [SerializeField]
        private CollisionTracker _attackArea;
        
        private readonly List<uint> _victims = new List<uint>();
        private AttackService _attackService;
        private TickTimer _cooldownTimer;
        private uint _attackerId;
        private float _cooldown;

        public void Setup(uint attackerId, float cooldown)
        {
            _attackerId = attackerId;
            _cooldown = cooldown;
        }

        public override void Spawned()
        {
            _attackService = ServiceLocator.instance.Get<AttackService>();
            
            if (HasStateAuthority)
            {
                _attackArea.onTriggerEnter += OnVictimEntered;
                _attackArea.onTriggerExit += OnVictimExited;    
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            { 
                _attackArea.onTriggerEnter -= OnVictimEntered; 
                _attackArea.onTriggerExit -= OnVictimExited;
            }
        }
        
        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _cooldownTimer.ExpiredOrNotRunning(Runner))
            {
                Attack();
                _cooldownTimer = TickTimer.CreateFromSeconds(Runner, _cooldown);
            }
        }

        private void Attack()
        {
            if (_victims.Count == 0)
                return;
            
            var randomVictim = _victims[Random.Range(0, _victims.Count)];
            _attackService.MakeAttack(_attackerId, randomVictim);
        }
        
        private void OnVictimEntered(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (victimId.HasValue && !_victims.Contains(victimId.Value))
                _victims.Add(victimId.Value); 
        }

        private void OnVictimExited(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (victimId.HasValue && _victims.Contains(victimId.Value))
                _victims.Remove(victimId.Value);
        }
    }
}