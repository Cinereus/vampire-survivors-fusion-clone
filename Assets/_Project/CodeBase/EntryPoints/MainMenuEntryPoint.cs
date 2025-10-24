using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        private LoadSceneService _loadSceneService;

        public void StartGame()
        {
            _loadSceneService.LoadScene(SceneNames.GAME);
        }
        
        private void Awake()
        {
            _loadSceneService = ServiceLocator.instance.Get<LoadSceneService>();
        }
    }
}
