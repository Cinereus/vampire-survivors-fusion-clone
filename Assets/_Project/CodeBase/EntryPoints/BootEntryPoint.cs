using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : MonoBehaviour
    {
        private void Awake()
        {
            var services = ServiceLocator.instance;
            RegisterServices(services);
            Initialize(services);
        }
        
        private void RegisterServices(ServiceLocator services)
        {
            services.Register(new LoadSceneService());
            services.Register(new AssetProvider());
        }
        
        private void Initialize(ServiceLocator services)
        {
            services.Get<LoadSceneService>().LoadScene(SceneNames.MAIN_MENU);
        }
    }
}