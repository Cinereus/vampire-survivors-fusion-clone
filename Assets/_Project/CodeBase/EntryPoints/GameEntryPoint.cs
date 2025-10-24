using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Components;
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
        private RectTransform _hudPlaceholder;
        
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
            var services = ServiceLocator.instance;
            RegisterDependencies(services);
            InitializeGame(services);
        }

        private void InitializeGame(ServiceLocator services)
        {
            var hero = services.Get<GameFactory>().CreateHero(_heroType, _hudPlaceholder, Vector3.zero);
            _camera.GetComponent<CameraFollow>()?.SetTarget(hero.transform);
            _spawner.Setup(services.Get<EnemySpawnService>());
        }

        private void OnDestroy()
        {
            ServiceLocator.instance.ClearContext(ServiceContext.Game);
        }

        private void RegisterDependencies(ServiceLocator services)
        {
            services.Register(new HeroesModel(_heroesConfig));
            services.Register(new EnemiesModel(_enemiesConfig));
            services.Register(new GameFactory(services.Get<HeroesModel>(), services.Get<EnemiesModel>(),
                services.Get<AssetProvider>()));
            services.Register(new PlayerInputService(), ServiceContext.Game);
            services.Register(new ItemsService(services.Get<HeroesModel>()), ServiceContext.Game);
            services.Register(new AttackService(services.Get<HeroesModel>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);
            services.Register(new LootSpawnService(services.Get<GameFactory>()), ServiceContext.Game);
            services.Register(new EnemySpawnService(_camera, services.Get<GameFactory>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);
        }
    }
}