namespace Services.Ads
{
    public class AdsManagerStub : IAdsManager
    {
        public bool CanShowRewardedVideo { get { return false; } }
        public void ShowRewardedVideo() {}
        public void ShowInterstital() {}

        public Status Status { get { return Status.NotInitialized; } }
    }
}
