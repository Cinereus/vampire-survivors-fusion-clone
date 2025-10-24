using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class PlayerMovement : MonoBehaviour
    {
        [SerializeField]
        private Rigidbody2D _rb;
        
        [SerializeField]
        private SpriteRenderer _renderer;
        
        private HeroModel _data;
        private PlayerInputService _inputService;

        public void Setup(PlayerInputService inputService, HeroModel data)
        {
            _data = data;
            _inputService = inputService;
        }

        public void FixedUpdate()
        {
            Vector2 moveDir = _inputService.GetAxis();
            _renderer.flipX = moveDir.x > 0;
            _rb.MovePosition(_rb.position + moveDir * (_data.speed * Time.fixedDeltaTime));
        }
    }
}

