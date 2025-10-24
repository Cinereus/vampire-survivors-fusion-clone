using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class CameraFollow : MonoBehaviour
    {
        [SerializeField]
        private Vector2 _offset;

        private Transform _target;
    
        public void SetTarget(Transform target) => _target = target;
        
        private void LateUpdate()
        {
            if (_target == null)
                return;
            
            transform.position = new Vector3(_target.position.x, _target.position.y, transform.position.z);
        }
    }
}