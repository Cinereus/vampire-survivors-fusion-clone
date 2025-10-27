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
        
        private HeroModel _data;

        public void Setup(HeroModel data)
        {
            _data = data;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                Vector2 moveDir = input.axis;
                _renderer.flipX = moveDir.x > 0;
                _rb.MovePosition(_rb.position + moveDir * (_data.speed * Runner.DeltaTime));
            }
        }
    }
}

