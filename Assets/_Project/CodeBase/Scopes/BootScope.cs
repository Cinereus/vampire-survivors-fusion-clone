using CodeBase.EntryPoints;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Services.SaveLoad;
using CodeBase.Infrastructure;
using CodeBase.UI;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Scopes
{
    public class BootScope : LifetimeScope
    {
        [SerializeField]
        private UIManager _uiManagerPrefab;
        
        protected override void Configure(IContainerBuilder builder)
        {
             builder.Register<AssetProvider>(Lifetime.Singleton);
             builder.Register<NetworkProvider>(Lifetime.Singleton);
             builder.Register<UIFactory>(Lifetime.Singleton);
             builder.Register<LoadSceneService>(Lifetime.Singleton);
             builder.Register<MatchmakingService>(Lifetime.Singleton);
             builder.Register<PlayerData>(Lifetime.Singleton).As<ISaveLoadEntity>().AsSelf();
             builder.Register<ISaveLoadService, SaveLoadPrefsService>(Lifetime.Singleton);
             builder.RegisterComponentInNewPrefab(_uiManagerPrefab, Lifetime.Singleton);
             builder.RegisterEntryPoint<BootEntryPoint>();
        }
    }
}