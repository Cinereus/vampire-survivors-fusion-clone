using UnityEngine;

namespace CodeBase.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        private LoadSceneService _loadSceneService;

        public void Setup(LoadSceneService loadSceneService) => _loadSceneService = loadSceneService;

        public void OnQuitPressed() => _loadSceneService.LoadScene(SceneNames.MAIN_MENU);
    }
}