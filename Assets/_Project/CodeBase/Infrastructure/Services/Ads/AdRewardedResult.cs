namespace CodeBase.Infrastructure.Services.Ads
{
    public struct AdRewardedResult
    {
        public readonly bool isSuccess;
        public readonly int amount;
        public readonly string adUnit;
        public readonly string rewardName; 
            
        public AdRewardedResult(bool isSuccess, int amount = 0, string adUnit = "", string rewardName = "")
        {
            this.isSuccess = isSuccess;
            this.adUnit = adUnit;
            this.rewardName = rewardName;
            this.amount = amount;
        }
    }
}