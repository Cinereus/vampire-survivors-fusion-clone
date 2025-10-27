using System.Collections.Generic;
using CodeBase.GameLogic.Services;
using UnityEngine;
using Random = UnityEngine.Random;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class MeleeAttack : MonoBehaviour
    {
        [SerializeField]
        private CollisionTracker _attackArea;
        
        private readonly List<uint> _victims = new List<uint>();
        private AttackService _attackService;
        private bool _isAttack;
        private float _lastHitTime;
        private uint _id;
        private float _cooldown;

        public void Setup(uint id, float cooldown, AttackService attackService)
        {
            _id = id;
            _cooldown = cooldown;
            _attackService = attackService;
            _attackArea.onTriggerEnter += OnVictimEntered;
            _attackArea.onTriggerExit += OnVictimExited;
        }
        
        private void Update()
        {
            if (_victims.Count > 0 && Time.realtimeSinceStartup >= _lastHitTime)
            {
                _lastHitTime = Time.realtimeSinceStartup + _cooldown;
                Attack();
            }
        }

        private void Attack()
        { 
            var randomVictim = _victims[Random.Range(0, _victims.Count)];
            _attackService.MakeAttack(_id, randomVictim);
        }
        
        private void OnVictimEntered(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<Identifier>()?.id;
            if (victimId.HasValue && !_victims.Contains(victimId.Value))
                _victims.Add(victimId.Value); 
        }

        private void OnVictimExited(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<Identifier>()?.id;
            if (victimId.HasValue && _victims.Contains(victimId.Value))
                _victims.Remove(victimId.Value);
        }

        private void OnDestroy()
        {
            _attackArea.onTriggerEnter -= OnVictimEntered;
            _attackArea.onTriggerExit -= OnVictimExited;
        }
    }
}