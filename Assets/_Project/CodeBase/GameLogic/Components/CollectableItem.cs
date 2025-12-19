using CodeBase.Configs;
using CodeBase.GameLogic.Services;
using CodeBase.Infrastructure;
using CodeBase.Infrastructure.Services.Analytics;
using CodeBase.Infrastructure.Services.Analytics.Events;
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

        [Networked]
        private ItemType item { get; set; }
        
        public ItemType itemType => item;
        
        private ItemsService _itemsService;
        private IAnalyticsService _analytics;

        public void Initialize(ItemType type)
        {
            item = type;
        }

        public override void Spawned()
        {
            _analytics = BehaviourInjector.instance.Resolve<IAnalyticsService>();
            
            if (HasStateAuthority)
            {
                _itemsService = BehaviourInjector.instance.Resolve<ItemsService>();
                _tracker.onTriggerEnter += OnPicked;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (HasStateAuthority) 
                _tracker.onTriggerEnter -= OnPicked;
            
            _analytics.LogEvent(new ItemDisappearedStatEvent(Runner.SessionInfo.Name, Object.Id.Raw, itemType));
        }

        private void OnPicked(Collider2D picker)
        {
            var id = picker.GetComponent<NetworkBehaviour>()?.Object?.Id.Raw;
            if (id.HasValue && _itemsService.TryPickUpItem(id.Value, item, _count))
            {
                _analytics.LogEvent(new ItemPickedStatEvent(Runner.SessionInfo.Name, id.Value, Object.Id.Raw, itemType));
                Runner.Despawn(Object);
            }
        }
    }
}