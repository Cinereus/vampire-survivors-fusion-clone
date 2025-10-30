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

        private HeroModel _model;
        
        public void Setup(HeroModel model)
        {
            _model = model;
            speed = _model.speed;
        }

        public override void Spawned()
        {
            if (HasStateAuthority)
            {
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
                Vector2 moveDir = input.axis;
                _renderer.flipX = moveDir.x > 0;
                _rb.MovePosition(_rb.position + moveDir * (speed * Runner.DeltaTime));
            }
        }
        
        private void OnLevelIncreased(uint _) => speed = _model.speed;
    }
}

