using System.Collections.Generic;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.Services;

namespace CodeBase.GameLogic
{
    public class PlayerData : IService
    {
        public HeroType chosenHero;
        public readonly List<string> visitedRooms = new List<string>();

        public void Dispose() { }
    }
}