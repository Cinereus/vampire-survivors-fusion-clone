using CodeBase.GameLogic.Services.SaveLoad;
using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : IInitializable
    {
        private readonly MatchmakingService _matchmakingService;
        private readonly LoadSceneService _sceneService;
        private readonly ISaveLoadService _saveLoad;

        public BootEntryPoint(LoadSceneService sceneService, MatchmakingService matchmakingService,
            ISaveLoadService saveLoad)
        {
            _matchmakingService = matchmakingService;
            _sceneService = sceneService;
            _saveLoad = saveLoad;
        }
        
        public void Initialize()
        {
            _saveLoad.Load();
            _matchmakingService.Initialize();
            if (_sceneService.GetActiveScene().name != SceneNames.MAIN_MENU) 
                _sceneService.LoadScene(SceneNames.MAIN_MENU);
        }
    }
}