using System;
using CodeBase.GameLogic.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Attacks
{
    public class AttackProjectile : MonoBehaviour
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

        public void Setup(uint id, Vector3 targetPos, AttackService attackService)
        {
            _id = id;
            _targetPos = targetPos;
            _attackService = attackService;
            _lifeTimeBorder = Time.time + _lifeTime;
            _hurtBox.onTriggerEnter += OnVictimEntered;
        }

        public void Update()
        {
            var targetDir = (_targetPos - transform.position).normalized;
            transform.position += targetDir * (_moveSpeed * Time.deltaTime);
            
            if (Time.time >= _lifeTimeBorder) 
                Destroy(gameObject);
        }

        private void OnVictimEntered(Collider2D victim)
        {
            var victimId = victim.transform.root.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (victimId.HasValue)
            {
                _attackService.MakeAttack(_id, victimId.Value);
                Destroy(gameObject);
            }
        }
        
        private void OnDestroy()
        {
            _hurtBox.onTriggerEnter -= OnVictimEntered;
        }
    }
}