using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
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
        private float speed { get; set; }
        
        [Networked]
        private Vector2 moveDir { get; set; }

        private HeroModel _model;
        
        public void Setup(HeroModel model)
        {
            _model = model;
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
                speed = _model.speed;
                _model.onLevelIncreased += OnLevelIncreased;
            }
        }
        
        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _model.onLevelIncreased -= OnLevelIncreased;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                moveDir = input.axis;
                _rb.MovePosition(_rb.position + moveDir * (speed * Runner.DeltaTime));
            }
            
            _renderer.flipX = moveDir.x > 0;
        }
        
        private void OnLevelIncreased(uint _) => speed = _model.speed;
    }
}

