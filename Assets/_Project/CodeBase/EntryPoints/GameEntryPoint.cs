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
        
        private void Start()
        {
            var services = ServiceLocator.instance;
            RegisterServices(services);
            InitializeGameSession(services);
            
            foreach (var no in FindObjectsByType<NetworkObject>(FindObjectsSortMode.None))
            {
                Debug.Log($"NO: name={no.name} id={no.GetInstanceID()} owner={no.HasStateAuthority} root={no.transform.root.name}");
            }
        }

        private void RegisterServices(ServiceLocator services)
        {
            services.Register(new HeroesModel(services.Get<GameSettingsProvider>()), ServiceContext.Game);
            
            services.Register(new EnemiesModel(services.Get<GameSettingsProvider>()), ServiceContext.Game);

            services.Register(new HeroesInstanceProvider(services.Get<HeroesModel>(), services.Get<NetworkContainer>()),
                ServiceContext.Game);
            
            services.Register(new GameFactory(services.Get<HeroesModel>(), services.Get<EnemiesModel>(),
                services.Get<AssetProvider>(), _uiPlaceholder, services.Get<NetworkContainer>()), 
                ServiceContext.Game);
            
            services.Register(new PlayerInputService(services.Get<NetworkContainer>()), ServiceContext.Game);
            
            services.Register(new ItemsService(services.Get<HeroesModel>()), ServiceContext.Game);
            
            services.Register(new AttackService(services.Get<HeroesModel>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);
            
            services.Register(new LootSpawnService(services.Get<GameFactory>()), ServiceContext.Game);

            services.Register(new HeroSpawnService(services.Get<GameFactory>(), services.Get<NetworkContainer>(),
                    services.Get<HeroesInstanceProvider>()), ServiceContext.Game);

            services.Register(new EnemySpawnService(_camera, services.Get<GameFactory>(),
                services.Get<HeroesInstanceProvider>(), services.Get<EnemiesModel>()),
                ServiceContext.Game);
        }

        private void InitializeGameSession(ServiceLocator services)
        {
            var playerData = services.Get<PlayerData>();
            var sceneService = services.Get<LoadSceneService>();
            var matchmakingService = services.Get<MatchmakingService>();
            var currentSceneIndex = SceneRef.FromIndex(sceneService.GetActiveSceneIndex());
            matchmakingService.StartGameSession(playerData.roomName, playerData.isHost, currentSceneIndex,
                onFailed: () => sceneService.LoadScene(SceneNames.MAIN_MENU));
            
            services.Get<NetworkContainer>().callbacks.onPlayerJoined += InitializeSpawners;
        }
        
        private void InitializeSpawners(NetworkRunner _, PlayerRef p)
        {
            var factory = ServiceLocator.instance.Get<GameFactory>();
            factory.CreateHeroSpawner(p);
            factory.CreateEnemySpawner();
            ServiceLocator.instance.Get<NetworkContainer>().callbacks.onPlayerJoined -= InitializeSpawners;
        }
        
        private void OnDestroy()
        {
            ServiceLocator.instance.Get<MatchmakingService>().KillSession();
            ServiceLocator.instance.ClearContext(ServiceContext.Game);
        }
    }
}