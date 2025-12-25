using System;

namespace CodeBase.Infrastructure.Services.Ads
{
    public interface IAdsService : IDisposable
    {
        public event Action<string, string, int> onRewarded;
        public void Initialize();
        public bool CanShowRewarded(string placement);
        public bool CanShowInterstitial(string placement);
        public void ShowRewarded(string placement);
        public void ShowInterstitial(string placement);
    }
}