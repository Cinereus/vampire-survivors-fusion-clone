using System;
using System.Collections.Generic;
using CodeBase.GameLogic.Models;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class RemoteAttack : MonoBehaviour
    {
        [SerializeField]
        private CollisionTracker _attackArea;
        
        private readonly List<Transform> _targets = new List<Transform>();
        private GameFactory _factory;
        private float _lastHitTime;
        private uint _id;
        private IAttackData _data;

        public void Setup(uint id, IAttackData data, GameFactory factory)
        {
            _id = id;
            _data = data;
            _factory = factory;
            _attackArea.onTriggerEnter += OnTargetEntered;
            _attackArea.onTriggerExit += OnTargetExited;
        }
        
        private void Update()
        {
            if (_targets.Count > 0 && Time.realtimeSinceStartup >= _lastHitTime)
            {
                _lastHitTime = Time.realtimeSinceStartup + _data.attackCooldown;
                Attack();
            }
        }

        private void Attack()
        { 
            var randomTarget = _targets[Random.Range(0, _targets.Count)];
            _factory.CreateProjectile(_id, transform.position, randomTarget.position);
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

        private void OnDestroy()
        {
            _attackArea.onTriggerEnter -= OnTargetEntered;
            _attackArea.onTriggerExit -= OnTargetExited;
        }
    }
}