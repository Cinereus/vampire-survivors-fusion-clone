using TMPro;
using UnityEngine;

namespace CodeBase.UI
{
    public class ProgressBar : MonoBehaviour
    {
        [SerializeField] 
        private TextMeshProUGUI _text;
        
        [SerializeField] 
        private RectTransform _fill;

        public void SetProgress(string text, float current, float max)
        {
            _text.text = text;
            _fill.localScale = new Vector3(current/max, _fill.localScale.y, _fill.localScale.z);
        }
    }
}
