using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine.SceneManagement;

namespace CodeBase
{
    public class LoadSceneService : IService
    {
        public void LoadScene(string sceneName) => SceneManager.LoadScene(sceneName);

        public int GetActiveSceneIndex() => SceneManager.GetActiveScene().buildIndex;
        
        public void Dispose() { }
    }
}