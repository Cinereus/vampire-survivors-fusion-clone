using CodeBase.Infrastructure.Services.Analytics.Events;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public interface IAnalyticsService
    {
        public void Initialize();
        public void LogEvent(IAnalyticsEvent statEvent);
    }
}