using CodeBase.Configs;
using CodeBase.GameLogic.Models;

namespace CodeBase.GameLogic.Services
{
    public class ItemsService
    {
        private readonly Heroes _heroes;

        public ItemsService(Heroes heroes)
        {
            _heroes = heroes;
        }

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