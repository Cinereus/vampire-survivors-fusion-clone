using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Components.Network;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class BootEntryPoint : MonoBehaviour
    {
        [SerializeField]
        private NetworkRunner _networkRunner;
        
        [SerializeField] 
        private NetworkRunnerCallbacks _networkCallbacks;
        
        [SerializeField] 
        private EnemiesConfig _enemiesConfig;

        [SerializeField] 
        private HeroesConfig _heroesConfig;
        
        private void Awake()
        {
            var services = ServiceLocator.instance;
            RegisterServices(services);
            Initialize(services);
        }
        
        private void RegisterServices(ServiceLocator services)
        {
            services.Register(new AssetProvider());
            services.Register(new LoadSceneService());
            services.Register(new NetworkContainer(_networkRunner, _networkCallbacks));
            services.Register(new GameSettingsProvider(_heroesConfig, _enemiesConfig));
        }
        
        private void Initialize(ServiceLocator services)
        {
            var network = services.Get<NetworkContainer>();
            network.runner.AddCallbacks(network.callbacks);
            
            services.Get<LoadSceneService>().LoadScene(SceneNames.MAIN_MENU);
        }
    }
}