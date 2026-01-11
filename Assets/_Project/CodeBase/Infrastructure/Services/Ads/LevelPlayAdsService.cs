using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Ads
{
    public class LevelPlayAdsService : IAdsService
    {
#if UNITY_ANDROID
        private const string APP_KEY = "24b00bfad";
#elif UNITY_IPHONE || UNITY_IOS
        private const string APP_KEY = "24b00f935";
#else
        private const string APP_KEY = "unxepected_platform";
#endif

        private readonly Dictionary<string, string> _placeToAdUnit = new Dictionary<string, string>
        {
            [AdsPlacements.LEVEL_START] = AdsUnitIds.REWARDED_GAME_SESSION_START,
        };

        private readonly Dictionary<string, ILevelPlayRewardedAd> _rewardedMap =
            new Dictionary<string, ILevelPlayRewardedAd>();

        private readonly Dictionary<string, ILevelPlayInterstitialAd> _interstitialMap =
            new Dictionary<string, ILevelPlayInterstitialAd>();

        private bool _isInited;

        private AutoResetUniTaskCompletionSource<AdRewardedResult> _showRewardedTask;
        private AutoResetUniTaskCompletionSource<bool> _showInterstitialTask;

        public void Initialize()
        {
            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;
            LevelPlay.Init(APP_KEY);
        }

        public bool CanShowRewarded(string placement)
        {
            return _isInited && !LevelPlayRewardedAd.IsPlacementCapped(placement) &&
                   _placeToAdUnit.TryGetValue(placement, out var adUnit) &&
                   _rewardedMap.TryGetValue(adUnit, out var rewardedAd) && rewardedAd.IsAdReady();
        }
        
        public bool CanShowInterstitial(string placement)
        {
            return _isInited && !LevelPlayInterstitialAd.IsPlacementCapped(placement) &&
                   _placeToAdUnit.TryGetValue(placement, out var adUnit) &&
                   _interstitialMap.TryGetValue(adUnit, out var interstitialAd) && interstitialAd.IsAdReady();
        }

        public async UniTask<AdRewardedResult> ShowRewarded(string placement)
        {
            if (_showRewardedTask != null)
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Previous ad is still showing.");
                return new AdRewardedResult(false);
            }
            
            if (!_placeToAdUnit.TryGetValue(placement, out var adUnit))
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Placement {placement} not registered.");
                return new AdRewardedResult(false);
            }

            if (_rewardedMap.TryGetValue(adUnit, out var ad))
            {
                if (ad.IsAdReady())
                {
                    _showRewardedTask = AutoResetUniTaskCompletionSource<AdRewardedResult>.Create();
                    var task = _showRewardedTask.Task;
                    ad.ShowAd(placement);
                    return await task;
                }
                    
                Debug.Log($"[{nameof(LevelPlayAdsService)}] Ad unit {adUnit} not ready.");
                return new AdRewardedResult(false, adUnit: adUnit);
            }
            
            Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] There is no ad unit for placement {placement}.");
            return new AdRewardedResult(false);
        }

        public async UniTask<bool> ShowInterstitial(string placement)
        {
            if (_showInterstitialTask != null)
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Previous ad is still showing.");
                return false;
            }
            
            if (!_placeToAdUnit.TryGetValue(placement, out var adUnit))
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Placement {placement} not registered.");
                return false;
            }

            if (_interstitialMap.TryGetValue(adUnit, out var ad))
            {
                if (ad.IsAdReady())
                {
                    _showInterstitialTask = AutoResetUniTaskCompletionSource<bool>.Create();
                    var task = _showInterstitialTask.Task;
                    ad.ShowAd(placement);
                    return await task;
                }
                Debug.Log($"[{nameof(LevelPlayAdsService)}] Ad unit {adUnit} not ready.");
                return false;
            }
  
            Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] There is no ad unit for placement {placement}.");
            return false;
        }
        
        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= OnInitSuccess;
            LevelPlay.OnInitFailed -= OnInitFailed;

            foreach (var interstitial in _interstitialMap.Values)
            {
                interstitial.OnAdLoaded -= OnInterstitialAdLoaded;
                interstitial.OnAdClosed -= OnInterstitialAdClosed;
                interstitial.OnAdLoadFailed -= OnInterstitialAdLoadFailed;
                interstitial.OnAdDisplayFailed -= OnInterstitialAdDisplayFailed;
            }
            
            foreach (var rewarded in _rewardedMap.Values)
            {
                rewarded.OnAdLoaded -= OnRewardedAdLoaded;
                rewarded.OnAdLoadFailed -= OnRewardedAdLoadFailed;
                rewarded.OnAdRewarded -= OnRewarded;
                rewarded.OnAdDisplayFailed -= OnRewardedAdDisplayFailed;
                rewarded.OnAdClosed -= OnRewardedAdClosed;
            }
            
            _showRewardedTask?.TrySetCanceled();
            _showRewardedTask = null;
            _showInterstitialTask?.TrySetCanceled();
            _showInterstitialTask = null;
        }

        private void InitAds()
        {
            InitRewarded(AdsUnitIds.REWARDED_GAME_SESSION_START);
        }

        private void InitRewarded(string unitId)
        {
            var ad = new LevelPlayRewardedAd(unitId);
            ad.OnAdLoaded += OnRewardedAdLoaded;
            ad.OnAdLoadFailed += OnRewardedAdLoadFailed;
            ad.OnAdRewarded += OnRewarded;
            ad.OnAdDisplayFailed += OnRewardedAdDisplayFailed;
            ad.OnAdClosed += OnRewardedAdClosed;
            ad.LoadAd();
            _rewardedMap[unitId] = ad;
        }

        private void InitInterstitial(string unitId)
        {
            var ad = new LevelPlayInterstitialAd(unitId);
            ad.OnAdLoaded += OnInterstitialAdLoaded;
            ad.OnAdLoadFailed += OnInterstitialAdLoadFailed;
            ad.OnAdClosed += OnInterstitialAdClosed;
            ad.OnAdDisplayFailed += OnInterstitialAdDisplayFailed;
            ad.LoadAd();
            _interstitialMap[unitId] = ad;
        }
        
        private void OnInitSuccess(LevelPlayConfiguration _)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Ads service loaded successfully.");
            _isInited = true;
            InitAds();
        }

        private void OnInitFailed(LevelPlayInitError err)
        {
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Failed to load level play ads with code: {err.ErrorCode}\nMessage: \"{err.ErrorMessage}\"");
        }

        private void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            SetRewardedTaskCompleted(new AdRewardedResult(true, reward.Amount, adInfo.AdUnitId, reward.Name));
            Debug.Log($"[{nameof(LevelPlayAdsService)}] OnRewarded unit: \"{adInfo.AdUnitName}\" reward: {reward.Name} amount: {reward.Amount}");
        }

        private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Rewarded ad \"{adInfo.AdUnitId}\" loaded successfully.");
        }
        
        private void OnRewardedAdLoadFailed(LevelPlayAdError adError)
        {
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Rewarded ad \"{adError.AdUnitId}\" load failed. Error: {adError.ErrorMessage}");
        }

        private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Interstitial ad \"{adInfo.AdUnitId}\" loaded successfully.");
        }
        
        private void OnInterstitialAdLoadFailed(LevelPlayAdError adError)
        {
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Interstitial ad \"{adError.AdUnitId}\" load failed. Error: {adError.ErrorMessage}");
        }
        
        private void OnRewardedAdClosed(LevelPlayAdInfo adInfo)
        {
            SetRewardedTaskCompleted(new AdRewardedResult(false, adUnit: adInfo.AdUnitId));
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Rewarded ad \"{adInfo.AdUnitName}\" closed.");
        }

        private void OnRewardedAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError adError)
        {
            SetRewardedTaskCompleted(new AdRewardedResult(false, adUnit: adInfo.AdUnitId));
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Rewarded ad \"{adInfo.AdUnitName}\" failed to display. Error: {adError.ErrorMessage}.");
        }
        
        private void OnInterstitialAdClosed(LevelPlayAdInfo adInfo)
        {
            SetInterstitialTaskCompleted(true);
        }
        
        private void OnInterstitialAdDisplayFailed(LevelPlayAdInfo adInfo, LevelPlayAdError adError)
        {
            SetInterstitialTaskCompleted(false);
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Interstitial ad \"{adInfo.AdUnitName}\" failed to display. Error: {adError.ErrorMessage}.");
        }

        private void SetInterstitialTaskCompleted(bool isSuccess)
        {
            if (_showInterstitialTask == null)
                return;
            
            _showInterstitialTask.TrySetResult(isSuccess);
            _showInterstitialTask = null;
        }
        
        private void SetRewardedTaskCompleted(AdRewardedResult result)
        {
            if (_showRewardedTask == null)
                return;
            
            _showRewardedTask.TrySetResult(result);
            _showRewardedTask = null;
        }
    }
}