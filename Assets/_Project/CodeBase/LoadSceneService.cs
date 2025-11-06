using System.Threading.Tasks;
using CodeBase.Infrastructure.Services;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class LoadSceneService : IService
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public Task LoadSceneAsync(string sceneName)
        {
            var completeSource = new TaskCompletionSource<bool>();
            var operation = SceneManager.LoadSceneAsync(sceneName);
            if (operation != null)
                operation.completed += _ => completeSource.SetResult(true);
            else
                completeSource.SetResult(true);

            return completeSource.Task;
        }

        public Scene GetActiveScene() => SceneManager.GetActiveScene();

        public void Dispose() { }
    }
}