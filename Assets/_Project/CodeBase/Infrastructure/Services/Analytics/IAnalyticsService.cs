using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.Services.Analytics
{
    public interface IAnalyticsService
    {
        public UniTask Initialize();
        public void LogEvent(string eventName, params (string name, string val)[] parameters);
    }
}