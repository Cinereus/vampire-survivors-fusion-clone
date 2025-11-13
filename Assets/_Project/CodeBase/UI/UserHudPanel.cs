using UnityEngine;

namespace CodeBase.UI
{
    public class UserHudPanel : BaseUIEntity
    {
        [SerializeField]
        private UserHud _userHud;

        public void UpdateXpValue(float currentXp, float maxXp) => _userHud.UpdateXpValue(currentXp, maxXp);

        public void UpdateHealthValue(float currentHealth, float maxHealth) => 
            _userHud.UpdateHealthValue(currentHealth, maxHealth);
    }
}