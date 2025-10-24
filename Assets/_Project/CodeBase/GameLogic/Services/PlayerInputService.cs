using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class PlayerInputService : IService
    {
        public Vector2 GetAxis() => new(SimpleInput.GetAxis("Horizontal"), SimpleInput.GetAxis("Vertical"));

        public void Dispose() { }
    }
}