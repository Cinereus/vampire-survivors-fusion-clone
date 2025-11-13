using CodeBase.GameLogic.Models;

namespace CodeBase.GameLogic.Services
{
    public class AttackService
    {
        private readonly EnemiesModel _enemies;
        private readonly HeroesModel _heroes;

        public AttackService(HeroesModel heroes, EnemiesModel enemies)
        {
            _heroes = heroes;
            _enemies = enemies;
        }

        public void MakeAttack(uint attackerId, uint victimId)
        {
            if (TryGetHeroModel(attackerId, out var hero))
            {
                var victim = _enemies.GetBy(victimId); 
                victim?.TakeDamage(hero.damage);    
            }
            else
            {
                var attacker = _enemies.GetBy(attackerId);
                var victim = _heroes.GetBy(victimId);
                victim?.TakeDamage(attacker.damage);
            }
        }
        
        private bool TryGetHeroModel(uint id, out HeroModel heroModel)
        {
            heroModel = _heroes.GetBy(id);
            return heroModel != null;
        }

    }
}