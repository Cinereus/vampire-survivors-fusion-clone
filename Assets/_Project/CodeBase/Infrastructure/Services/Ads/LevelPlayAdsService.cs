using System;
using System.Collections.Generic;
using Unity.Services.LevelPlay;
using UnityEngine;

namespace CodeBase.Infrastructure.Services.Ads
{
    public class LevelPlayAdsService : IAdsService
    {
        public event Action<string, string, int> onRewarded;

#if UNITY_ANDROID
        private const string APP_KEY = "24b00bfad";
#elif UNITY_IPHONE || UNITY_IOS
        private const string APP_KEY = "24b00f935";
#else
        private const string APP_KEY = "unxepected_platform";
#endif

        private readonly Dictionary<string, string> _placeToAdUnit = new Dictionary<string, string>
        {
            [AdsPlacementNames.LEVEL_START] = AdsUnitIds.REWARDED_GAME_SESSION_START,
        };

        private readonly Dictionary<string, ILevelPlayRewardedAd> _rewardedMap =
            new Dictionary<string, ILevelPlayRewardedAd>();

        private readonly Dictionary<string, ILevelPlayInterstitialAd> _interstitialMap =
            new Dictionary<string, ILevelPlayInterstitialAd>();

        private bool _isInited;

        public void Initialize()
        {
            LevelPlay.OnInitSuccess += OnInitSuccess;
            LevelPlay.OnInitFailed += OnInitFailed;
            LevelPlay.Init(APP_KEY);
        }

        public bool CanShowRewarded(string placement)
        {
            return _isInited && _placeToAdUnit.TryGetValue(placement, out var adUnit) &&
                   _rewardedMap.TryGetValue(adUnit, out var rewardedAd) && rewardedAd.IsAdReady();
        }
        
        public bool CanShowInterstitial(string placement)
        {
            return _isInited && _placeToAdUnit.TryGetValue(placement, out var adUnit) &&
                   _interstitialMap.TryGetValue(adUnit, out var interstitialAd) && interstitialAd.IsAdReady();
        }

        public void ShowRewarded(string placement)
        {
            if (!_placeToAdUnit.TryGetValue(placement, out var adUnit))
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Placement {placement} not registered.");
                return;
            }

            if (_rewardedMap.TryGetValue(adUnit, out var ad) && ad.IsAdReady())
            {
                if (ad.IsAdReady())
                    ad.ShowAd(placement);
                else
                    Debug.Log($"[{nameof(LevelPlayAdsService)}] Ad unit {adUnit} not ready.");
            }
            else
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] There is no ad unit for placement {placement}.");
            }
        }

        public void ShowInterstitial(string placement)
        {
            if (!_placeToAdUnit.TryGetValue(placement, out var adUnit))
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] Placement {placement} not registered.");
                return;
            }

            if (_interstitialMap.TryGetValue(adUnit, out var ad))
            {
                if (ad.IsAdReady())
                    ad.ShowAd(placement);
                else
                    Debug.Log($"[{nameof(LevelPlayAdsService)}] Ad unit {adUnit} not ready.");
            }
            else
            {
                Debug.LogWarning($"[{nameof(LevelPlayAdsService)}] There is no ad unit for placement {placement}.");
            }
        }
        
        public void Dispose()
        {
            LevelPlay.OnInitSuccess -= OnInitSuccess;
            LevelPlay.OnInitFailed -= OnInitFailed;

            foreach (var interstitial in _interstitialMap.Values) 
                interstitial.OnAdLoaded -= OnInterstitialAdLoaded;

            foreach (var rewarded in _rewardedMap.Values)
            {
                rewarded.OnAdLoaded -= OnRewardedAdLoaded;
                rewarded.OnAdRewarded -= OnRewarded;
            }
        }
        
        private void OnInitSuccess(LevelPlayConfiguration obj)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Ads service loaded successfully.");
            _isInited = true;
            InitAds();
        }

        private void OnInitFailed(LevelPlayInitError err) =>
            Debug.LogError($"[{nameof(LevelPlayAdsService)}] Failed to load level play ads with code: {err.ErrorCode}\nMessage: \"{err.ErrorMessage}\"");

        private void InitAds()
        {
            InitRewarded(AdsUnitIds.REWARDED_GAME_SESSION_START);
        }

        private void InitRewarded(string unitId)
        {
            var ad = new LevelPlayRewardedAd(unitId);
            ad.OnAdLoaded += OnRewardedAdLoaded;
            ad.OnAdRewarded += OnRewarded;
            ad.LoadAd();
            _rewardedMap[unitId] = ad;
        }

        private void InitInterstitial(string unitId)
        {
            var ad = new LevelPlayInterstitialAd(unitId);
            ad.OnAdLoaded += OnInterstitialAdLoaded;
            ad.LoadAd();
            _interstitialMap[unitId] = ad;
        }

        private void OnRewarded(LevelPlayAdInfo adInfo, LevelPlayReward reward)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] OnRewarded unit: \"{adInfo.AdUnitName}\" reward: {reward.Name} amount: {reward.Amount}");
            onRewarded?.Invoke(adInfo.AdUnitName, reward.Name, reward.Amount);
        }

        private void OnRewardedAdLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Rewarded ad \"{adInfo.AdUnitName}\" loaded successfully.");
        }

        private void OnInterstitialAdLoaded(LevelPlayAdInfo adInfo)
        {
            Debug.Log($"[{nameof(LevelPlayAdsService)}] Interstitial ad \"{adInfo.AdUnitName}\" loaded successfully.");
        }
    }
}