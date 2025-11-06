using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Services
{
    public class PlayerInputService : IInitializeService
    {
        private readonly NetworkProvider _network;

        public PlayerInputService(NetworkProvider network)
        {
            _network = network;
        }

        public void Initialize() => _network.callbacks.onInput += OnInput;

        public void Dispose() => _network.callbacks.onInput -= OnInput;

        private void OnInput(NetworkRunner runner, NetworkInput input) =>
            input.Set(new NetworkInputData { axis = GetAxis() });

        private Vector2 GetAxis() => new(SimpleInput.GetAxis("Horizontal"), SimpleInput.GetAxis("Vertical"));
    }
}