using CodeBase.UI;
using Cysharp.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class LoadSceneService
    {
        private readonly UIManager _uiManager;

        public LoadSceneService(UIManager uiManager)
        {
            _uiManager = uiManager;
        }
        
        public void LoadScene(string sceneName, bool needShowLoadingScreen = true)
        {
            if (needShowLoadingScreen)
                _uiManager.ShowLoadingScreen();
            
            SceneManager.LoadScene(sceneName);
            
            if (needShowLoadingScreen)
                _uiManager.HideLoadingScreen();
        }

        public async UniTask LoadSceneAsync(string sceneName, bool needShowLoading = true)
        {
            if (needShowLoading)
                _uiManager.ShowLoadingScreen();
            
            await SceneManager.LoadSceneAsync(sceneName).ToUniTask();
            
            if (needShowLoading) 
                _uiManager.HideLoadingScreen();
        }

        public Scene GetActiveScene() => SceneManager.GetActiveScene();

    }
}