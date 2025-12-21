using CodeBase.Infrastructure.AssetManagement;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CodeBase.UI
{
    public class UIFactory
    {
        private readonly AssetProvider _assetProvider;

        public UIFactory(AssetProvider assetProvider)
        {
            _assetProvider = assetProvider;
        }

        public T CreateUIEntity<T>(RectTransform parent) where T : BaseUIEntity
        {
            BaseUIEntity prefab = _assetProvider.GetUIEntity<T>();
            if (prefab == null)
            {
                Debug.LogError($"{nameof(UIFactory)} failed to create UIEntity of type {typeof(T)}");
                return null;
            }
            
            var newInstance = Object.Instantiate(prefab, parent);
            return newInstance.GetComponent<T>();
        }
    }
}