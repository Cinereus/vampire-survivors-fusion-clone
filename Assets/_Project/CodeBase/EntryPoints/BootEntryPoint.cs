using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : MonoBehaviour
    {
        private void Awake()
        {
            var services = ServiceLocator.instance;
            services.RegisterService(new LoadSceneService());
            services.GetService<LoadSceneService>().LoadScene(SceneNames.MAIN_MENU);
        }
    }
}