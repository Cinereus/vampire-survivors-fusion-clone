using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake() => DontDestroyOnLoad(this);
    }
}