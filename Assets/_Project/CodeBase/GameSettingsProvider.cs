using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;

namespace CodeBase
{
    public class GameSettingsProvider : IService
    {
        public readonly HeroesConfig heroesConfig;
        public readonly EnemiesConfig enemiesConfig;

        public GameSettingsProvider(HeroesConfig heroesConfig, EnemiesConfig enemiesConfig)
        {
            this.heroesConfig = heroesConfig;
            this.enemiesConfig = enemiesConfig;
        }
        
        public void Dispose() { }
    }
}