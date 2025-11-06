using CodeBase.Configs;
using CodeBase.GameLogic.Models;
using CodeBase.Infrastructure.Services;

namespace CodeBase.GameLogic.Services
{
    public class ItemsService : IService
    {
        private readonly HeroesModel _heroes;

        public ItemsService(HeroesModel heroes)
        {
            _heroes = heroes;
        }
        
        public void Dispose() { }

        public bool TryPickUpItem(uint id, ItemType item, float count)
        {
            if (_heroes.TryGetBy(id, out var hero))
            {
                hero.PickUp(item, count);
                return true;
            }
            return false;
        }
    }
}