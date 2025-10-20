using CodeBase.Infrastructure.Services;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class LoadSceneService : IService
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public void Dispose() { }
    }
}