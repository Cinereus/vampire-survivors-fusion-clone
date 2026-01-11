using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.AssetManagement.Loaders
{
    public interface IAssetLoader : IDisposable
    {
        public UniTask<List<T>> LoadGroupAsync<T>(string key) where T : class;
        public UniTask<T> LoadAsync<T>(string key) where T : class;
        public void Release(string key);
    }
}