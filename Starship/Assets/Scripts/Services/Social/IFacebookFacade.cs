using Utils;

namespace Services.Social
{
    public interface IFacebookFacade
    {
        bool IsRewardedPostAvailable { get; }
        void Share();
    }

    public class FacebookShareCompletedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
