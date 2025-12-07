using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace CodeBase.GameLogic.Services.SaveLoad
{
    public class SaveLoadPrefsService : ISaveLoadService
    {
        private IEnumerable<ISaveLoadEntity> entities => _resolver.Resolve<IEnumerable<ISaveLoadEntity>>();
        private readonly IObjectResolver _resolver;

        public SaveLoadPrefsService(IObjectResolver resolver)
        {
            _resolver = resolver;
        }
        
        public PlayerData LoadPlayerData() =>
            JsonUtility.FromJson<PlayerData>(PlayerPrefs.GetString(PrefKeys.PLAYER_DATA));

        public void Save()
        {
            var data = new SaveLoadData();
            foreach (var entity in entities) 
                entity.Save(ref data);
            
            PlayerPrefs.SetString(PrefKeys.PLAYER_DATA, JsonUtility.ToJson(data));
            Debug.Log($"[{nameof(SaveLoadPrefsService)}] Data saved.");
        }

        public void Load()
        {
            string json = PlayerPrefs.GetString(PrefKeys.PLAYER_DATA);
            if (string.IsNullOrEmpty(json))
                return;
            
            var data = JsonUtility.FromJson<SaveLoadData>(json);
            foreach (var entity in entities) 
                entity.Load(data);
            
            Debug.Log($"[{nameof(SaveLoadPrefsService)}] Data loaded.");
        }
    }
}