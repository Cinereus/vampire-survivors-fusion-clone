using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class MainMenuEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private NetworkRunner _networkRunner;
        
        [SerializeField] 
        private NetworkRunnerCallbacks _networkCallbacks;
        
        private LoadSceneService _loadSceneService;

        public void StartGame() => _loadSceneService.LoadScene(SceneNames.GAME);

        private void Awake()
        {
            var services = ServiceLocator.instance;
            services.Register(new NetworkContainer(_networkRunner, _networkCallbacks));
            _loadSceneService = services.Get<LoadSceneService>();
        }
    }
}
