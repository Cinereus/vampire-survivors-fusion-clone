using System.Collections.Generic;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class HeroRemoteAttack : NetworkBehaviour
    {
        [SerializeField]
        private CollisionTracker _attackArea;

        [Networked]
        private float attackCooldown { get; set; }
        
        private readonly List<Transform> _targets = new List<Transform>();
        private GameFactory _factory;
        private HeroModel _model;
        private TickTimer _cooldownTimer;

        public void Setup(HeroModel model)
        {
            _model = model;
            attackCooldown = _model.attackCooldown;
        }

        public override void Spawned()
        {
            _factory = ServiceLocator.instance.Get<GameFactory>();
            
            if (HasStateAuthority)
            {
                _attackArea.onTriggerEnter += OnTargetEntered;
                _attackArea.onTriggerExit += OnTargetExited;
                _model.onLevelIncreased += OnLevelIncreased;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _attackArea.onTriggerEnter -= OnTargetEntered;
                _attackArea.onTriggerExit -= OnTargetExited;    
                _model.onLevelIncreased -= OnLevelIncreased;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _cooldownTimer.ExpiredOrNotRunning(Runner))
            { 
                Attack();
                _cooldownTimer = TickTimer.CreateFromSeconds(Runner, attackCooldown);
            }
        }
        
        private void Attack()
        {
            if (_targets.Count == 0)
                return;
            
            var randomTarget = _targets[Random.Range(0, _targets.Count)];
            _factory.CreateProjectile(_model.id, transform.position, randomTarget.position);    
        }
        
        private void OnTargetEntered(Collider2D target)
        { 
            if (!_targets.Contains(target.transform))
                _targets.Add(target.transform); 
        }

        private void OnTargetExited(Collider2D target)
        {
            if (_targets.Contains(target.transform)) 
                _targets.Remove(target.transform);
        }
        
        private void OnLevelIncreased(uint _)
        {
            attackCooldown = _model.attackCooldown;
        }
    }
}