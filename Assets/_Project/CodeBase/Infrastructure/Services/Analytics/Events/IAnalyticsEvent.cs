using System.Collections.Generic;

namespace CodeBase.Infrastructure.Services.Analytics.Events
{
    public interface IAnalyticsEvent
    {
        string eventName { get; }
        Dictionary<string, string> parameters { get; }
    }
}