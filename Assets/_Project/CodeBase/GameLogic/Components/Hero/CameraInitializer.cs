using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components.Hero
{
    public class CameraInitializer : NetworkBehaviour 
    {
        public override void Spawned()
        {
            if (HasInputAuthority) 
                Camera.main?.GetComponent<CameraFollow>()?.SetTarget(transform);
        }
    }
}