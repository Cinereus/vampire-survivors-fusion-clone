using System;
using Cysharp.Threading.Tasks;

namespace CodeBase.Infrastructure.Services.Ads
{
    public interface IAdsService : IDisposable
    {
        public void Initialize();
        public bool CanShowRewarded(string placement);
        public bool CanShowInterstitial(string placement);
        public UniTask<AdRewardedResult> ShowRewarded(string placement);
        public UniTask<bool> ShowInterstitial(string placement);
    }
}