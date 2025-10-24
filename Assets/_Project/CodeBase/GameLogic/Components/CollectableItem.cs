using CodeBase.Configs;
using CodeBase.GameLogic.Services;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class CollectableItem : MonoBehaviour
    {
        [SerializeField] 
        private float _count;
        
        [SerializeField]
        private CollisionTracker _tracker;

        private ItemType _item;
        private ItemsService _itemsService;

        public void Setup(ItemType item, ItemsService itemService)
        {
            _item = item;
            _itemsService = itemService;
            _tracker.onTriggerEnter += OnPicked;
        }

        private void OnDestroy()
        {
            _tracker.onTriggerEnter -= OnPicked;
        }

        private void OnPicked(Collider2D picker)
        {
            var id = picker.GetComponent<Identifier>()?.id;
            if (id.HasValue && _itemsService.TryPickUpItem(id.Value, _item, _count)) 
                Destroy(gameObject);
        }
    }
}