using CodeBase.GameLogic;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services;
using Fusion;
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
        private GameObject _loadingScreen;

        private GameFactory _factory;
        private NetworkProvider _network;

        private void Awake()
        {
            var services = ServiceLocator.instance;
            RegisterServices(services);
            SetupDependencies(services);
            SubscribeToEvents();
        }

        private void RegisterServices(ServiceLocator services)
        {
            services.Register(new HeroesModel(services.Get<GameSettingsProvider>().heroesConfig.heroes),
                ServiceContext.Game);

            services.Register(new EnemiesModel(services.Get<GameSettingsProvider>().enemiesConfig.enemies),
                ServiceContext.Game);

            services.Register(new HeroesInstanceProvider(), ServiceContext.Game);

            services.Register(new GameFactory(services.Get<HeroesModel>(), services.Get<EnemiesModel>(),
                services.Get<AssetProvider>(), _uiPlaceholder, services.Get<NetworkProvider>()), ServiceContext.Game);

            services.Register(new PlayersLeftService(services.Get<HeroesModel>(), 
                services.Get<NetworkProvider>().callbacks, services.Get<HeroesInstanceProvider>()),
                ServiceContext.Game);
            
            services.Register(new PlayerInputService(services.Get<NetworkProvider>()), ServiceContext.Game);

            services.Register(new ItemsService(services.Get<HeroesModel>()), ServiceContext.Game);

            services.Register(new AttackService(services.Get<HeroesModel>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);

            services.Register(new LootSpawnService(services.Get<GameFactory>()), ServiceContext.Game);

            services.Register(new HeroSpawnService(services.Get<GameFactory>(), services.Get<HeroesInstanceProvider>()),
                ServiceContext.Game);

            services.Register(new EnemySpawnService(_camera, services.Get<GameFactory>(), 
                services.Get<HeroesInstanceProvider>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);


            services.Register(new HostMigrationService(services.Get<MatchmakingService>(), services.Get<GameFactory>(),
                    services.Get<NetworkProvider>().callbacks, services.Get<HeroesModel>(),
                    services.Get<EnemiesModel>(), services.Get<HeroesInstanceProvider>()), ServiceContext.Game);
        }

        private void SetupDependencies(ServiceLocator services)
        {
            _factory = services.Get<GameFactory>();
            _network = services.Get<NetworkProvider>();
        }

        private void SubscribeToEvents()
        {
            _network.callbacks.onHostMigration += OnHostMigration;
            _network.callbacks.onSceneLoadDone += OnSceneLoadDone;
            _network.callbacks.onShutdown += OnShutdown;
            _network.callbacks.onPlayerJoined += OnPlayerJoined;
        }
        
        private void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
        {
            if (runner.LocalPlayer == player)
                _factory.CreateHeroSpawner();
        }

        private void OnSceneLoadDone(NetworkRunner runner)
        {
            _loadingScreen.SetActive(false);
            _factory.CreateEnemySpawner();
        }

        private void OnHostMigration(NetworkRunner runner, HostMigrationToken token)
        { 
            _loadingScreen.SetActive(true);
        }

        private void OnShutdown(NetworkRunner runner, ShutdownReason reason)
        {
            _factory.CreateGameOverScreenLocal(_loadingScreen);
        }

        private void OnDestroy()
        {
            _network.callbacks.onHostMigration -= OnHostMigration;
            _network.callbacks.onSceneLoadDone -= OnSceneLoadDone;
            _network.callbacks.onShutdown -= OnShutdown;
            _network.callbacks.onPlayerJoined -= OnPlayerJoined;
            ServiceLocator.instance.ClearContext(ServiceContext.Game);
        }
    }
}