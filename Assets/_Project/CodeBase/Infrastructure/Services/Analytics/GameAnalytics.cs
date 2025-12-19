using CodeBase.Configs;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public class GameAnalytics
    {
        private const string SESSION_NAME = "sessionName";
        private const string NET_OBJECT_ID = "netObjId";
        private const string ITEM_TYPE = "itemType";
        
        private readonly IAnalyticsService _analytics;
        
        public GameAnalytics(IAnalyticsService analytics) => _analytics = analytics;

        public void SendItemPicked(string sessionName, uint pickerId, uint objId, ItemType itemType)
        {
            _analytics.LogEvent(AnalyticsKeys.ITEM_PICKED, parameters: new []
            { 
                (name: SESSION_NAME, val: sessionName),
                (name: "pickerId", val: pickerId.ToString()),
                (name: NET_OBJECT_ID, val: objId.ToString()),
                (name: ITEM_TYPE, val: itemType.ToString()),
            });
        }
        
        public void SendItemDisappear(string sessionName, uint objId, ItemType itemType)
        {
            _analytics.LogEvent(AnalyticsKeys.ITEM_DISAPPEARED,  parameters: new []
            {
                (name: SESSION_NAME, val: sessionName),
                (name: NET_OBJECT_ID, val: objId.ToString()),
                (name: ITEM_TYPE, val: itemType.ToString()),
            });
        }
        
        public void SendGameSessionStarted(string sessionName, bool isSuccess, bool isMigration)
        {
            _analytics.LogEvent(AnalyticsKeys.GAME_SESSION_STARTED,
                parameters: new[]
                {
                    (name: SESSION_NAME, val: sessionName),
                    (name: "isSuccess", val: isSuccess.ToString()),
                    (name: "isMigration", val: isMigration.ToString()),
                });
        }
        
        public void SendGameSessionFinished(string sessionName) => 
            _analytics.LogEvent(AnalyticsKeys.GAME_SESSION_FINISHED,  (name: SESSION_NAME, val: sessionName));
    }
}