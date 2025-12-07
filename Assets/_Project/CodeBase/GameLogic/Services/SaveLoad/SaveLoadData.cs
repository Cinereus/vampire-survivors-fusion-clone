using System;
using CodeBase.Configs.Heroes;

namespace CodeBase.GameLogic.Services.SaveLoad
{
    [Serializable]
    public struct SaveLoadData
    {
        public HeroType chosenHero;
    }
}