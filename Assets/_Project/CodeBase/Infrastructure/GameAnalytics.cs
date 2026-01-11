using CodeBase.Configs;
using CodeBase.Infrastructure.Services.Analytics;

namespace CodeBase.Infrastructure
{
    public class GameAnalytics
    {
        private const string NONE = "None";
        private const string SESSION_NAME = "sessionName";
        private const string NET_OBJ_ID = "netObjId";
        private const string ITEM_TYPE = "itemType";
        private const string PICKER_ID = "pickerId";
        private const string IS_SUCCESS = "isSuccess";
        private const string IS_MIGRATION = "isMigration";

        private readonly NetworkProvider _network;
        private readonly IAnalyticsService _analytics;
        
        private string sessionName => _network.runner.SessionInfo?.Name ?? NONE;

        public GameAnalytics(IAnalyticsService analytics, NetworkProvider network)
        {
            _network = network;
            _analytics = analytics;
        }

        public void SendItemPicked(uint pickerId, uint objId, ItemType itemType)
        {
            _analytics.LogEvent(AnalyticsKeys.ITEM_PICKED, parameters: new []
            { 
                (name: SESSION_NAME, val: sessionName),
                (name: PICKER_ID, val: pickerId.ToString()),
                (name: NET_OBJ_ID, val: objId.ToString()),
                (name: ITEM_TYPE, val: itemType.ToString()),
            });
        }
        
        public void SendItemDisappear(uint objId, ItemType itemType)
        {
            _analytics.LogEvent(AnalyticsKeys.ITEM_DISAPPEARED,  parameters: new []
            {
                (name: SESSION_NAME, val: sessionName),
                (name: NET_OBJ_ID, val: objId.ToString()),
                (name: ITEM_TYPE, val: itemType.ToString()),
            });
        }
        
        public void SendGameSessionStarted(bool isSuccess, bool isMigration)
        {
            _analytics.LogEvent(AnalyticsKeys.GAME_SESSION_STARTED,
                parameters: new[]
                {
                    (name: SESSION_NAME, val: sessionName),
                    (name: IS_SUCCESS, val: isSuccess.ToString()),
                    (name: IS_MIGRATION, val: isMigration.ToString()),
                });
        }
        
        public void SendGameSessionFinished() => 
            _analytics.LogEvent(AnalyticsKeys.GAME_SESSION_FINISHED,  (name: SESSION_NAME, val: sessionName));
    }
}