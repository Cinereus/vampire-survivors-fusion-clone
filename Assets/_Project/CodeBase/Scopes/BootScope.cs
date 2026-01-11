using CodeBase.EntryPoints;
using CodeBase.Infrastructure.AssetManagement.Loaders;
using CodeBase.Infrastructure.AssetManagement;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.GameLogic.Services.SaveLoad;
using CodeBase.GameLogic;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services.Ads;
using CodeBase.Infrastructure.Services.Configs;
using VContainer.Unity;
using CodeBase.UI;
using UnityEngine;
using VContainer;

namespace CodeBase.Scopes
{
    public class BootScope : LifetimeScope
    {
        [SerializeField] 
        private UIManager _uiManagerPrefab;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<IAssetLoader, AddressableAssetLoader>(Lifetime.Singleton);
            builder.Register<AssetProvider>(Lifetime.Singleton);
            builder.Register<IConfigProvider, FirebaseConfigProvider>(Lifetime.Singleton);
            builder.Register<NetworkProvider>(Lifetime.Singleton);
            builder.Register<UIFactory>(Lifetime.Singleton);
            builder.Register<LoadSceneService>(Lifetime.Singleton);
            builder.Register<PlayerData>(Lifetime.Singleton).As<ISaveLoadEntity>().AsSelf();
            builder.Register<IAdsService, LevelPlayAdsService>(Lifetime.Singleton);
            builder.Register<ISaveLoadService, SaveLoadPrefsService>(Lifetime.Singleton);
            builder.Register<IAnalyticsService, FirebaseAnalyticsService>(Lifetime.Singleton);
            builder.Register<GameAnalytics>(Lifetime.Singleton);
            builder.Register<MatchmakingService>(Lifetime.Singleton);
            builder.RegisterComponentInNewPrefab(_uiManagerPrefab, Lifetime.Singleton);
            builder.RegisterEntryPoint<BootEntryPoint>();
        }
    }
}