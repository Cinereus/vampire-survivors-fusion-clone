using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        private LoadSceneService _loadSceneService;

        private void Awake()
        {
            _loadSceneService = ServiceLocator.instance.GetService<LoadSceneService>();
        }

        public void StartGame()
        {
            _loadSceneService.LoadScene(SceneNames.GAME);
        }
    }
}
