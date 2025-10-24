using System;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class CollisionTracker : MonoBehaviour
    {
        public event Action<Collider2D> onTriggerEnter;
        public event Action<Collider2D> onTriggerExit;
        
        private void OnTriggerEnter2D(Collider2D other) => onTriggerEnter?.Invoke(other);
        private void OnTriggerExit2D(Collider2D other) => onTriggerExit?.Invoke(other);
    }
}
