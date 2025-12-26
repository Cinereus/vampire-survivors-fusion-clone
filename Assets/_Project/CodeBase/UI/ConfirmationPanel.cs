using System;
using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class ConfirmationPanel : BaseUIEntity
    {
        [SerializeField] 
        private TextMeshProUGUI _text;
        
        private Action _onNoPressed;
        private Action _onYesPressed;

        public void Initialize(string text, Action onYesPressed, Action onNoPressed)
        {
            _text.text = text;
            _onNoPressed = onNoPressed;
            _onYesPressed = onYesPressed;
        }
        
        public void OnNoPressed() => _onNoPressed?.Invoke();
        public void OnYesPressed() => _onYesPressed?.Invoke();

        public override void Hide()
        {
            _text.text = string.Empty;
            base.Hide();
        }
    }
}