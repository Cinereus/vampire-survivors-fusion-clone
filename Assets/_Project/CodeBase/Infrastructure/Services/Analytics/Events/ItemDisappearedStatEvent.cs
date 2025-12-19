using System.Collections.Generic;
using CodeBase.Configs;

namespace CodeBase.Infrastructure.Services.Analytics.Events
{
    public class ItemDisappearedStatEvent : IAnalyticsEvent
    {
        public string eventName => AnalyticsKeys.ITEM_DISAPPEARED;
        public Dictionary<string, string> parameters { get; }

        public ItemDisappearedStatEvent(string sessionName, uint objId, ItemType type)
        {
            parameters = new Dictionary<string, string>
            {
                ["sessionName"] = sessionName, 
                ["netObjId"] = objId.ToString(), 
                ["itemType"] = type.ToString(),
            };
        }
    }
}