using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CodeBase.Configs;
using CodeBase.Configs.Enemies;
using CodeBase.Configs.Heroes;
using CodeBase.Infrastructure.AssetManagement.Loaders;
using CodeBase.UI;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.Infrastructure.AssetManagement
{
    public class AssetProvider : IDisposable
    {
        private const string GAME_GROUP = "Game";
        private const string COMMON_GROUP = "Common";
        private const string MAIN_MENU_GROUP = "MainMenu";
        
        private readonly Dictionary<string, Object> _gameAssets = new Dictionary<string, Object>();
        private readonly Dictionary<string, Object> _commonAssets = new Dictionary<string, Object>();
        private readonly IAssetLoader _loader;

        public AssetProvider(IAssetLoader loader)
        {
            _loader = loader;
        }

        public async Task PrepareCommonAssetGroup()
        {
            List<Object> assets = new List<Object>();
            assets.AddRange(await _loader.LoadGroupAsync<Object>(COMMON_GROUP));
            assets.AddRange(await _loader.LoadGroupAsync<Object>(MAIN_MENU_GROUP));

            foreach (var asset in assets)
                _commonAssets[asset.name] = asset;
        }

        public async Task PrepareGameAssetGroup()
        {
            List<Object> gameAssets = await _loader.LoadGroupAsync<Object>(GAME_GROUP);
            foreach (var asset in gameAssets)
                _gameAssets[asset.name] = asset;
        }

        public void ReleaseGameAssetGroup()
        {
            _loader.Release(GAME_GROUP);
            _gameAssets.Clear();
        }

        public void Dispose()
        {
            _loader.Release(COMMON_GROUP);
            _loader.Release(MAIN_MENU_GROUP);
            _commonAssets.Clear();
        }

        public TConfig GetConfig<TConfig>(string name = "") where TConfig : ScriptableObject
        {
            if (string.IsNullOrEmpty(name))
                name = typeof(TConfig).Name;

            return (TConfig) GetCommonAsset(name);
        }

        public TEntity GetUIEntity<TEntity>() where TEntity : BaseUIEntity
        {
            var name = typeof(TEntity).Name;
            if ((_commonAssets.TryGetValue(name, out var o) || _gameAssets.TryGetValue(name, out o)) && o is GameObject result)
                return result.GetComponent<TEntity>();

            Debug.LogWarning($"[{nameof(AssetProvider)}] UUEntity with name {name} not found");
            return null;
        }

        public GameObject GetNetworkRunner() => (GameObject) GetCommonAsset("NetworkRunner");

        public GameObject GetNetworkCallbacks() => (GameObject) GetCommonAsset("NetworkCallbacks");

        public GameObject GetEnemySpawner() => (GameObject) GetGameAsset("EnemySpawner");

        public GameObject GetHeroSpawner() => (GameObject) GetGameAsset("HeroSpawner");

        public GameObject GetEnemy(EnemyType type) => (GameObject) GetGameAsset($"{type}");

        public GameObject GetItem(ItemType type) => (GameObject) GetGameAsset($"{type}");

        public GameObject GetHero(HeroType type) => (GameObject) GetGameAsset($"{type}");

        public GameObject GetProjectile(HeroType type) => (GameObject) GetGameAsset($"{type}Projectile");

        private Object GetCommonAsset(string name)
        {
            if (_commonAssets.TryGetValue(name, out var result))
                return result;

            Debug.LogWarning($"[{nameof(AssetProvider)}] {name} not found");
            return null;
        }
        
        private Object GetGameAsset(string name)
        {
            if (_gameAssets.TryGetValue(name, out var result))
                return result;

            Debug.LogWarning($"[{nameof(AssetProvider)}] {name} not found");
            return null;
        }
    }
}