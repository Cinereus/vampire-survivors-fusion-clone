using UnityEngine;

namespace CodeBase.Configs
{
    public struct ConfigDefValues
    {
        public readonly TextAsset heroesDefValues;
        public readonly TextAsset enemiesDefValues;

        public ConfigDefValues(TextAsset heroesDefValues, TextAsset enemiesDefValues)
        {
            this.heroesDefValues = heroesDefValues;
            this.enemiesDefValues = enemiesDefValues;
        }
    }
}