using CodeBase.GameLogic.Models;

namespace CodeBase.GameLogic.Services
{
    public class AttackService
    {
        private readonly Heroes _heroes;
        private readonly Enemies _enemies;

        public AttackService(Heroes heroes, Enemies enemies)
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