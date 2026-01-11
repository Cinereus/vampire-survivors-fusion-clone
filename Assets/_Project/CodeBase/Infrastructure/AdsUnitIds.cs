namespace CodeBase.Infrastructure
{
    public static class AdsUnitIds
    {
#if UNITY_ANDROID
        public const string REWARDED_GAME_SESSION_START = "k452tvis5uxc6mkh";
#elif UNITY_IOS || UNITY_IPHONE
        public const string REWARDED_GAME_SESSION_START = "0ke98daka7ivmsfv";
#else
        public const string REWARDED_GAME_SESSION_START = "mock_game_session_start";
#endif
    }
}