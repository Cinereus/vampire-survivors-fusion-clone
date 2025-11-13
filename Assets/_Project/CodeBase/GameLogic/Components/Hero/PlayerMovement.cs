using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class PlayerMovement : NetworkBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        
        [SerializeField]
        private SpriteRenderer _renderer;
        
        [Networked]
        private Vector2 moveDir { get; set; }
        
        [Networked]
        private float speed { get; set; }
        
        private HeroModel _model;
        
        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                _model = BehaviourInjector.instance.Resolve<HeroesModel>().GetBy(Object.Id.Raw);
                _model.onLevelIncreased += OnLevelIncreased;
                speed = _model.speed;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
                _model.onLevelIncreased -= OnLevelIncreased;
        }

        private void OnLevelIncreased(uint id)
        {
            if (_model.id == id) 
                speed = _model.speed;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                moveDir = input.axis;
                _rb.MovePosition(_rb.position + moveDir * (speed * Runner.DeltaTime));
            }
        }

        public override void Render()
        {
            _renderer.flipX = moveDir.x > 0;
        }
    }
}

