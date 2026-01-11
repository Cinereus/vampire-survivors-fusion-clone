using System;
using System.Collections.Generic;

namespace CodeBase.Configs.Heroes
{
    [Serializable]
    public class HeroesConfig : IConfig
    {
        public List<HeroData> heroes;
    }
}