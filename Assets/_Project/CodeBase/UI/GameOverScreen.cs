using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        private LoadSceneService _loadSceneService;

        public void Setup() => _loadSceneService = ServiceLocator.instance.Get<LoadSceneService>();

        public void OnQuitPressed() => _loadSceneService.LoadScene(SceneNames.MAIN_MENU);
    }
}