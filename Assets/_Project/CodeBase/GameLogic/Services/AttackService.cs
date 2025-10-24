using System;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;

namespace CodeBase.GameLogic.Services
{
    public class AttackService : IService
    {
        private readonly EnemiesModel _enemies;
        private readonly HeroesModel _heroes;

        public AttackService(HeroesModel heroes, EnemiesModel enemies)
        {
            _heroes = heroes;
            _enemies = enemies;
        }

        public void MakeAttack(Guid attackerId, Guid victimId)
        {
            if (TryGetHeroModel(attackerId, out var hero))
            {
                var victim = _enemies.GetEnemyBy(victimId); 
                victim.TakeDamage(hero.damage);    
            }
            else
            {
                var attacker = _enemies.GetEnemyBy(attackerId);
                var victim = _heroes.GetHeroBy(victimId);
                victim.TakeDamage(attacker.damage);
            }
        }
        
        public void Dispose() { }
        
        private bool TryGetHeroModel(Guid id, out HeroModel heroModel)
        {
            heroModel = _heroes.GetHeroBy(id);
            return heroModel != null;
        }

    }
}