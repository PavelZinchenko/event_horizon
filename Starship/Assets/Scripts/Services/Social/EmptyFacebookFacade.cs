namespace Services.Social
{
    public class EmptyFacebookFacade : IFacebookFacade
    {
        public bool IsRewardedPostAvailable { get { return false; } }
        public void Share() {}
    }
}
