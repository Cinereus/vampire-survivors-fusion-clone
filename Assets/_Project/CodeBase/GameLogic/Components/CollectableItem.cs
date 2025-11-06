using CodeBase.Configs;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure.Services;
using Fusion;
using UnityEngine;

namespace CodeBase.GameLogic.Components
{
    public class CollectableItem : NetworkBehaviour
    {
        [SerializeField] 
        private float _count;
        
        [SerializeField]
        private CollisionTracker _tracker;

        public ItemType itemType => item;
        
        [Networked]
        private ItemType item { get; set; }
        
        private ItemsService _itemsService;

        public void Setup(ItemType type)
        {
            item = type;
        }

        public override void Spawned()
        {
            _itemsService = ServiceLocator.instance.Get<ItemsService>();
            
            if (HasStateAuthority)
            {
                _tracker.onTriggerEnter += OnPicked;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority)
            {
                _tracker.onTriggerEnter -= OnPicked;
            }
        }

        private void OnPicked(Collider2D picker)
        {
            var id = picker.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (id.HasValue && _itemsService.TryPickUpItem(id.Value, item, _count)) 
                Runner.Despawn(Object);
        }
    }
}