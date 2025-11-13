using CodeBase.EntryPoints;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace CodeBase.Scopes
{
    public class MainMenuScope : LifetimeScope
    {
        [SerializeField]
        private Camera _mainCamera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<MainMenuEntryPoint>(Lifetime.Scoped).WithParameter(_mainCamera);
        }
    }
}