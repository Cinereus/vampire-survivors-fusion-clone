using System.Collections.Generic;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure;
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
        private TickTimer _cooldownTimer;
        private GameFactory _factory;
        private HeroModel _model;
        
        public override void Spawned()
        {
            SetupDependencies();

            if (HasStateAuthority)
            {
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
        
        private void SetupDependencies()
        {
            _factory = BehaviourInjector.instance.Resolve<GameFactory>();
            _model = BehaviourInjector.instance.Resolve<HeroesModel>().GetBy(Object.Id.Raw);
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