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
            services.Register(new LoadSceneService());
            services.Register(new AssetProvider());
            services.Get<LoadSceneService>().LoadScene(SceneNames.MAIN_MENU);
        }
    }
}