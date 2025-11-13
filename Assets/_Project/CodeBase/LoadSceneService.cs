using System.Threading.Tasks;
using CodeBase.UI;
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

        public Task LoadSceneAsync(string sceneName, bool needShowLoadingScreen = true)
        {
            if (needShowLoadingScreen)
                _uiManager.ShowLoadingScreen();
            
            var completeSource = new TaskCompletionSource<bool>();
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null)
            {
                operation.completed += _ =>
                {
                    completeSource.SetResult(true);
                    
                    if (needShowLoadingScreen)
                        _uiManager.HideLoadingScreen();
                };
            }
            else
            {
                completeSource.SetResult(true);
                
                if (needShowLoadingScreen)
                    _uiManager.HideLoadingScreen();
            }
            return completeSource.Task;
        }

        public Scene GetActiveScene() => SceneManager.GetActiveScene();

    }
}