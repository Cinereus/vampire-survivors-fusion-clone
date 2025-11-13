using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : IInitializable
    {
        private readonly MatchmakingService _matchmakingService;
        private readonly LoadSceneService _sceneService;

        public BootEntryPoint(LoadSceneService sceneService, MatchmakingService matchmakingService)
        {
            _matchmakingService = matchmakingService;
            _sceneService = sceneService;
        }
        
        public void Initialize()
        {
            _matchmakingService.Initialize();
            
            if (_sceneService.GetActiveScene().name != SceneNames.MAIN_MENU) 
                _sceneService.LoadScene(SceneNames.MAIN_MENU);
        }
    }
}