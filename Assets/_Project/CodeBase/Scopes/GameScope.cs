using CodeBase.EntryPoints;
using CodeBase.GameLogic;
using CodeBase.GameLogic.Models;
using CodeBase.GameLogic.Services;
using UnityEngine;
using VContainer.Unity;
using VContainer;

namespace CodeBase.Scopes
{
    public class GameScope : LifetimeScope
    {
        [SerializeField]
        private Camera _camera;
        
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<Heroes>(Lifetime.Scoped);
            builder.Register<Enemies>(Lifetime.Scoped);
            builder.Register<HeroesInstanceProvider>(Lifetime.Scoped);
            builder.Register<GameFactory>(Lifetime.Scoped);
            builder.Register<ItemsService>(Lifetime.Scoped);
            builder.Register<AttackService>(Lifetime.Scoped);
            builder.Register<LootSpawnService>(Lifetime.Scoped);
            builder.Register<HeroSpawnService>(Lifetime.Scoped);
            builder.Register<EnemySpawnService>(Lifetime.Scoped);
            builder.Register<NetObjectRestoreService>(Lifetime.Scoped);
            builder.UseEntryPoints(entryPoints =>
            {
                entryPoints.Add<HostMigrationService>();
                entryPoints.Add<PlayersLeftService>();
                entryPoints.Add<PlayerInputService>();
                entryPoints.Add<GameEntryPoint>().WithParameter(_camera);    
            });
            
        }
    }
}