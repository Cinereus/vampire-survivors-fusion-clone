using CodeBase.Infrastructure.Services;
using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class GameOverScreen : MonoBehaviour
    {
        [SerializeField]
        public Button _quitButton;
        
        private MatchmakingService _matchmakingService;
        private LoadSceneService _loadSceneService;
        private GameObject _loadingScreen;

        public void Setup(GameObject loadingScreen)
        {
            _loadingScreen = loadingScreen;
            _matchmakingService = ServiceLocator.instance.Get<MatchmakingService>();
            _loadSceneService = ServiceLocator.instance.Get<LoadSceneService>();
        }

        public void OnQuitPressed()
        {
            _quitButton.interactable = false;   
            GoToMainMenu();
        }

        private async void GoToMainMenu()
        {
            _loadingScreen.SetActive(true);
            await _matchmakingService.KillSession();
            await _loadSceneService.LoadSceneAsync(SceneNames.MAIN_MENU);
        }
    }
}