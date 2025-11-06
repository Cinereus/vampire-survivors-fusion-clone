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
        
        private readonly List<Transform> _targets = new List<Transform>();
        private GameFactory _factory;
        private HeroesModel _heroes;
        private TickTimer _cooldownTimer;
        private HeroModel _model;

        public override void Spawned()
        {
            _factory = ServiceLocator.instance.Get<GameFactory>();
            _heroes = ServiceLocator.instance.Get<HeroesModel>();
            
            if (HasStateAuthority && _heroes.TryGetBy(Object.Id.Raw, out var model))
            {
                _model = model;
                _attackArea.onTriggerEnter += OnTargetEntered;
                _attackArea.onTriggerExit += OnTargetExited;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _attackArea.onTriggerEnter -= OnTargetEntered;
                _attackArea.onTriggerExit -= OnTargetExited;    
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (HasStateAuthority && _cooldownTimer.ExpiredOrNotRunning(Runner))
            { 
                Attack();
                _cooldownTimer = TickTimer.CreateFromSeconds(Runner, _model.attackCooldown);
            }
        }
        
        private void Attack()
        {
            if (_targets.Count == 0)
                return;
            
            var randomTarget = _targets[Random.Range(0, _targets.Count)];
            _factory.CreateProjectile(Object.Id.Raw, transform.position, randomTarget.position);    
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
    }
}