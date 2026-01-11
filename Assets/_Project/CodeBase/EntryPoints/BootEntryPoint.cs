using System.Threading;
using CodeBase.GameLogic.Services.SaveLoad;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.Ads;
using CodeBase.Infrastructure.Services.Analytics;
using Cysharp.Threading.Tasks;
using VContainer.Unity;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : IAsyncStartable
    {
        private readonly MatchmakingService _matchmakingService;
        private readonly LoadSceneService _sceneService;
        private readonly AssetProvider _assetProvider;
        private readonly IAnalyticsService _analytics;
        private readonly ISaveLoadService _saveLoad;
        private readonly NetworkProvider _network;
        private readonly IAdsService _ads;

        public BootEntryPoint(LoadSceneService sceneService, MatchmakingService matchmakingService,
            ISaveLoadService saveLoad, IAnalyticsService analytics, NetworkProvider network,
            AssetProvider assetProvider, IAdsService ads)
        {
            _matchmakingService = matchmakingService;
            _assetProvider = assetProvider;
            _sceneService = sceneService;
            _analytics = analytics;
            _network = network;
            _saveLoad = saveLoad;
            _ads = ads;
        }

        public async UniTask StartAsync(CancellationToken _)
        {
            await _assetProvider.PrepareCommonAssetGroup();
            _saveLoad.Load();
            _ads.Initialize();
            _network.Initialize();
            _analytics.Initialize();
            _matchmakingService.Initialize();
            
            if (_sceneService.GetActiveScene().name != SceneNames.MAIN_MENU) 
              await _sceneService.LoadSceneAsync(SceneNames.MAIN_MENU);
        }
    }
}