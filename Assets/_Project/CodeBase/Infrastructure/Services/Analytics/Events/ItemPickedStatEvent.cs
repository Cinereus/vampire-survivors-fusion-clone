using System.Collections.Generic;
using CodeBase.Configs;

namespace CodeBase.Infrastructure.Services.Analytics.Events
{
    public class ItemPickedStatEvent : IAnalyticsEvent
    {
        public string eventName => AnalyticsKeys.ITEM_PICKED;
        public Dictionary<string, string> parameters { get; }

        public ItemPickedStatEvent(string sessionName, uint pickerId, uint objId, ItemType type)
        {
            parameters = new Dictionary<string, string>
            {
                ["sessionName"] = sessionName, 
                ["pickerId"] = pickerId.ToString(),
                ["netObjId"] = objId.ToString(), 
                ["itemType"] = type.ToString(),
            };
        }
    }
}