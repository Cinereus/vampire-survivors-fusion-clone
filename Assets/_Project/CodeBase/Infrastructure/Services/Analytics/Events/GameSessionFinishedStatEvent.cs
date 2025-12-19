using System.Collections.Generic;

namespace CodeBase.Infrastructure.Services.Analytics.Events
{
    public class GameSessionFinishedStatEvent : IAnalyticsEvent
    {
        public string eventName => AnalyticsKeys.GAME_SESSION_FINISHED;
        public Dictionary<string, string> parameters { get; }

        public GameSessionFinishedStatEvent(string sessionName) => parameters = new Dictionary<string, string>
            { ["sessionName"] = sessionName };
    }
}