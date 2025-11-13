using System.Collections.Generic;
using CodeBase.Configs.Heroes;

namespace CodeBase.GameLogic
{
    public class PlayerData
    {
        public HeroType chosenHero;
        public readonly List<string> visitedRooms = new List<string>();
    }
}