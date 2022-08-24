using Utils;

namespace Services.Ads
{
    public enum Status
    {
        NotInitialized,
        Busy,
        Ready,
    }

    public interface IAdsManager
    {
        void ShowRewardedVideo();
        void ShowInterstital();
        Status Status { get; }
    }

    public class RewardedVideoCompletedSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase {}
    }

    public class AdsManagerStatusChangedSignal : SmartWeakSignal<Status>
    {
        public class Trigger : TriggerBase { }
    }
}
