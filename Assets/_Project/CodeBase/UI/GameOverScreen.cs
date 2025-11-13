using UnityEngine;
using UnityEngine.UI;

namespace CodeBase.UI
{
    public class GameOverScreen : BaseUIEntity
    {
        [SerializeField]
        public Button _quitButton;
        
        private LoadSceneService _sceneService;
        private MatchmakingService _matchmakingService;
        
        public void Initialize(MatchmakingService matchmakingService, LoadSceneService sceneService)
        {
            _sceneService = sceneService;
            _matchmakingService = matchmakingService;
        }

        public void OnQuitPressed()
        {
            _quitButton.interactable = false;   
            GoToMainMenu();
        }

        private async void GoToMainMenu()
        {
            await _matchmakingService.KillSession();
            await _sceneService.LoadSceneAsync(SceneNames.MAIN_MENU);
        }
    }
}