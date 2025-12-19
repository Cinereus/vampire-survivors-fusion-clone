using System.Collections.Generic;

namespace CodeBase.Infrastructure.Services.Analytics.Events
{
    public class GameSessionStartedStatEvent : IAnalyticsEvent
    {
        public string eventName => AnalyticsKeys.GAME_SESSION_START;
        public Dictionary<string, string> parameters { get; }

        public GameSessionStartedStatEvent(string sessionName, bool isSuccess, bool isMigration)
        {
            parameters = new Dictionary<string, string>
            {
                ["sessionName"] = sessionName,
                ["isSuccess"] = isSuccess.ToString(),
                ["isMigration"] = isMigration.ToString(),
            };
        }
    }
}