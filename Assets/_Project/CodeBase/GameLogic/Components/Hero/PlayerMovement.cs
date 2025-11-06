using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
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

        private HeroesModel _heroes;
        private HeroModel _model;

        public override void Spawned()
        {
            _heroes = ServiceLocator.instance.Get<HeroesModel>();
            
            if (_heroes.TryGetBy(Object.Id.Raw, out var model)) 
                _model = model;
        }

        public override void FixedUpdateNetwork()
        {
            if (GetInput(out NetworkInputData input))
            {
                moveDir = input.axis;
                _rb.MovePosition(_rb.position + moveDir * (_model.speed * Runner.DeltaTime));
            }
        }

        public override void Render()
        {
            _renderer.flipX = moveDir.x > 0;
        }
    }
}

