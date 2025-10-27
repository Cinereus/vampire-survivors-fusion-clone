using CodeBase.GameLogic.Models;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class EnemyDeathHandler : MonoBehaviour
    {
        private EnemyModel _model;

        public void Setup(EnemyModel model)
        {
            _model = model;
            _model.onDeath += OnDeath;
        }

        private void OnDeath(uint _) => Destroy(gameObject);
        
        private void OnDestroy()
        {
            if (_model != null) 
                _model.onDeath -= OnDeath;
        }
    }
}