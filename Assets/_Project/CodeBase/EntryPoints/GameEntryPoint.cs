using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Components.Enemy;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using UnityEngine;

namespace CodeBase.EntryPoints
{
    public class GameEntryPoint : MonoBehaviour
    {
        [SerializeField] 
        private Camera _camera;
        
        [SerializeField]
        private RectTransform _uiPlaceholder;
        
        [SerializeField] 
        private EnemySpawner _spawner;

        [SerializeField]
        private HeroType _heroType;

        [SerializeField] 
        private EnemiesConfig _enemiesConfig;

        [SerializeField] 
        private HeroesConfig _heroesConfig;
        
        private void Awake()
        {
            RegisterDependencies();
        }

        private void RegisterDependencies()
        {
            var services = ServiceLocator.instance;
            
            services.Register(new HeroesModel(_heroesConfig), ServiceContext.Game);
            
            services.Register(new EnemiesModel(_enemiesConfig), ServiceContext.Game);

            services.Register(new HeroesInstanceProvider(services.Get<HeroesModel>(), services.Get<NetworkContainer>()),
                ServiceContext.Game);
            
            services.Register(new GameFactory(services.Get<HeroesModel>(), services.Get<EnemiesModel>(),
                services.Get<AssetProvider>(), services.Get<HeroesInstanceProvider>(),
                _uiPlaceholder, services.Get<NetworkContainer>()), ServiceContext.Game);
            
            services.Register(new PlayerInputService(), ServiceContext.Game);
            
            services.Register(new ItemsService(services.Get<HeroesModel>()), ServiceContext.Game);
            
            services.Register(new AttackService(services.Get<HeroesModel>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);
            
            services.Register(new LootSpawnService(services.Get<GameFactory>()), ServiceContext.Game);

            services.Register(new HeroSpawnService(_camera, services.Get<GameFactory>(), 
                services.Get<NetworkContainer>(), services.Get<HeroesInstanceProvider>()),
                ServiceContext.Game);
            
            services.Register(new EnemySpawnService(_camera, services.Get<GameFactory>(), 
                services.Get<HeroesInstanceProvider>(), services.Get<EnemiesModel>(),
                services.Get<NetworkContainer>()), ServiceContext.Game);
        }
        
        private void OnDestroy() => ServiceLocator.instance.ClearContext(ServiceContext.Game);
    }
}