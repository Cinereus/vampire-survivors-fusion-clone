using System.Collections.Generic;
using UnityEngine;
using VContainer;

namespace CodeBase.UI
{
    public class UIManager : MonoBehaviour
    {
        [SerializeField]
        private Canvas _canvas;
        
        [SerializeField]
        private GameObject _loadingScreen;
        
        [SerializeField]
        private RectTransform _uiPlaceholder;

        public Camera actualCamera => _canvas.worldCamera; 
        
        private readonly Dictionary<string, BaseUIEntity> _instances = new Dictionary<string, BaseUIEntity>();
        private UIFactory _uiFactory;
       
        [Inject]
        public void Construct(UIFactory uiFactory)
        {
            _uiFactory = uiFactory;
        }

        public void SetupActualCamera(Camera cam)
        {
            _canvas.worldCamera = cam;
        }

        // todo Make something like viewModel layer instead of instance return
        public T Show<T>(bool needCache = false) where T : BaseUIEntity
        {
            var entityName = typeof(T).Name;
            if (_instances.TryGetValue(entityName, out BaseUIEntity entity))
            {
                entity.Show();
                return (T) entity;
            }

            var newInstance = _uiFactory.CreateUIEntity<T>(_uiPlaceholder);
            if (newInstance == null)
            {
                Debug.LogError($"{nameof(UIManager)}: Failed to create UIEntity of type {typeof(T)}");
                return null; 
            }
                
            newInstance.isNeedCache = needCache;
            _instances[entityName] = newInstance;
            return (T) _instances[entityName];
        }

        public void Hide<T>() where T : BaseUIEntity
        {
            var entityName = typeof(T).Name;
            if (_instances.TryGetValue(entityName, out var entity))
            {
                entity.Hide();
                if (!entity.isNeedCache)
                {
                    entity.Dispose();
                    _instances.Remove(entityName);                    
                }
            }
        }

        public void ShowLoadingScreen() => _loadingScreen.SetActive(true);

        public void HideLoadingScreen() => _loadingScreen.SetActive(false);
    }
}