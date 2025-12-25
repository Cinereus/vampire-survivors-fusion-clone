namespace CodeBase.Infrastructure
{
    public static class AdsUnitIds
    {
#if UNITY_ANDROID
        public const string REWARDED_GAME_SESSION_START = "k452tvis5uxc6mkh";
#endif
#if UNITY_IOS || UNITY_IPHONE
        public const string REWARDED_GAME_SESSION_START = "0ke98daka7ivmsfv";
#endif
    }
}