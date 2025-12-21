using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CodeBase.Infrastructure.AssetManagement.Loaders
{
    public interface IAssetLoader : IDisposable
    {
        public Task<List<T>> LoadGroupAsync<T>(string key) where T : class;
        public Task<T> LoadAsync<T>(string key) where T : class;
        public void Release(string key);
    }
}