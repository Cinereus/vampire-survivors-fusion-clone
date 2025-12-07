using System;
using System.Collections.Generic;
using CodeBase.Configs.Heroes;
using CodeBase.GameLogic.Services.SaveLoad;

namespace CodeBase.GameLogic
{
    [Serializable]
    public class PlayerData : ISaveLoadEntity
    {
        public HeroType chosenHero;
        public readonly List<string> visitedRooms = new List<string>();
        
        public void Save(ref SaveLoadData data) => data.chosenHero = chosenHero;
        public void Load(SaveLoadData data) => chosenHero = data.chosenHero;
    }
}