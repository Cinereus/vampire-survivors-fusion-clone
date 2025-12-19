namespace CodeBase.Infrastructure.Services.Analytics
{
    public interface IAnalyticsService
    {
        public void Initialize();
        public void LogEvent(string eventName, params (string name, string val)[] parameters);
    }
}