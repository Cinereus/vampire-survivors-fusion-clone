using System.Collections.Generic;
using UnityEngine;

namespace CodeBase.Configs.Heroes
{
    [CreateAssetMenu(fileName = "HeroesConfig", menuName = "Configs/HeroesConfig")]
    public class HeroesConfig : ScriptableObject
    {
        public List<HeroData> heroes;
    }
}