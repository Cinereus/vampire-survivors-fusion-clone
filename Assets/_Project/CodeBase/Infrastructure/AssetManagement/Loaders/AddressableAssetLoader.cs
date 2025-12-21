using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace CodeBase.Infrastructure.AssetManagement.Loaders
{
    public class AddressableAssetLoader : IAssetLoader
    {
        private readonly Dictionary<string, AsyncOperationHandle> _unreleasedHandles =
            new Dictionary<string, AsyncOperationHandle>();

        public async Task<List<T>> LoadGroupAsync<T>(string key) where T : class
        {
            if (_unreleasedHandles.TryGetValue(key, out var cachedHandle))
                return cachedHandle.Result as List<T>;
            
            var handle = Addressables.LoadAssetsAsync<T>(key);
            await handle.Task;
            
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _unreleasedHandles[key] = handle;
                return handle.Result.ToList();
            }
            
            throw new Exception($"[{nameof(AddressableAssetLoader)}] Failed to load asset group: {key}");
        }

        public async Task<T> LoadAsync<T>(string key) where T : class
        {
            if (_unreleasedHandles.TryGetValue(key, out var cachedHandle))
                return cachedHandle.Result as T;
            
            var handle = Addressables.LoadAssetAsync<T>(key);
            await handle.Task;

            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                _unreleasedHandles[key] = handle;
                return handle.Result;
            }
            
            throw new Exception($"[{nameof(AddressableAssetLoader)}] Failed to load asset: {key}");
        }
        
        public void Release(string key)
        {
            if (_unreleasedHandles.TryGetValue(key, out var handle))
            {
                Addressables.Release(handle);
                _unreleasedHandles.Remove(key);
            }
            else
            { 
                Debug.LogWarning($"[{nameof(AddressableAssetLoader)}] Failed to release asset by key {key}. Key not found.");
            }
        }
        
        public void Dispose()
        {
            foreach (var handle in _unreleasedHandles.Values) 
                Addressables.Release(handle);
            
            _unreleasedHandles.Clear();
        }
    }
}