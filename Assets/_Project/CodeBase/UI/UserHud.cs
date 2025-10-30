using UnityEngine;

namespace CodeBase.UI
{
    public class UserHud : MonoBehaviour
    {
        [SerializeField]
        private ProgressBar _healthBar;

        [SerializeField]
        private ProgressBar _xpBar;
        
        public void UpdateHealthValue(float current, float max) => 
            _healthBar.SetProgress($"{current}/{max}", current, max);

        public void UpdateXpValue(float current, float max) =>
            _xpBar.SetProgress($"{current}/{max}", current, max);
    }
}