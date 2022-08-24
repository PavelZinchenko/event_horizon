//using System;
//using Economy.Products;
//using Facebook.Unity;
//using GameServices.Economy;
//using GameServices.Gui;
//using GameServices.Settings;
//using Services.InternetTime;
//using Services.Localization;
//using Session;
//using Zenject;

//namespace Services.Social
//{
//    public class FacebookFacade : IInitializable, IDisposable, IFacebookFacade
//    {
//        [Inject] private readonly ILocalization _localization;
//        [Inject] private readonly FacebookShareCompletedSignal.Trigger _shareCompletedTrigger;
//        [Inject] private readonly ISessionData _session;
//        [Inject] private readonly GameSettings _gameSettings;
//        [Inject] private readonly LootGenerator _lootGenerator;
//        [Inject] private readonly GuiHelper _guiHelper;
//        [Inject] private readonly InternetTimeService _internetTime;

//        public void Initialize()
//        {
//            FB.Init(OnInitComplete);
//        }

//        public void Dispose()
//        {
//        }

//        public bool IsRewardedPostAvailable
//        {
//            get
//            {
//#if !UNITY_EDITOR
//                var currentDay = CurrentDay;
//                if (_session.Social.LastFacebookPostDate + FacebookPostCooldown >= currentDay)
//                    return false;
//                if (_gameSettings.LastFacebookPostDate + FacebookPostCooldown >= currentDay)
//                    return false;
//#endif
//                return true;
//            }
//        }

//        public void Share()
//        {
//            if (!_initialized)
//                return;

//#if UNITY_ANDROID
//            var link = new Uri("https://play.google.com/store/apps/details?id=" + AppConfig.bundleIdentifier);
//#elif UNITY_IPHONE
//            var link = new Uri("https://itunes.apple.com/app/id1098794574");
//#else
//            var link = new Uri("https://play.google.com/store/apps/details?id=" + AppConfig.bundleIdentifier);
//#endif

//            FB.ShareLink(
//                link,
//                _localization.GetString("$Credits_Title"),
//                _localization.GetString("$Credits_Title"),
//                new Uri("https://zipagames.com/icons/eventhorizon.png"),
//                ShareCallback);
//        }

//        private void OnInitComplete()
//        {
//            _initialized = FB.IsInitialized;
//            UnityEngine.Debug.Log("FacebookFacade: Initialized - " + _initialized);
//        }

//        private void ShareCallback(IShareResult result)
//        {
//            if (result == null)
//            {
//                UnityEngine.Debug.Log("FacebookFacade: Null Response");
//                return;
//            }

//            if (!string.IsNullOrEmpty(result.Error))
//            {
//                UnityEngine.Debug.Log("FacebookFacade: Error - result.Error");
//            }
//            else if (result.Cancelled)
//            {
//                UnityEngine.Debug.Log("FacebookFacade: Cancelled - result.RawResult");
//            }
//            else if (!string.IsNullOrEmpty(result.RawResult))
//            {
//                UnityEngine.Debug.Log("FacebookFacade: Success - result.RawResult");
//                OnRewardedPostCompleted();
//            }
//            else
//            {
//                UnityEngine.Debug.Log("FacebookFacade: Empty Response\n");
//            }
//        }

//        private void OnRewardedPostCompleted()
//        {
//            if (!IsRewardedPostAvailable)
//                return;

//            var currentDay = CurrentDay;
//            _session.Social.LastFacebookPostDate = currentDay;
//            _gameSettings.LastFacebookPostDate = currentDay;

//            var reward = _lootGenerator.GetSocialShareReward();
//            _guiHelper.ShowLootWindow(reward);
//            reward.Consume();

//            _shareCompletedTrigger.Fire();
//        }

//        private int CurrentDay
//        {
//            get
//            {
//                var currentTime = _internetTime.HasBeenReceived ? _internetTime.DateTime : DateTime.UtcNow;
//                return currentTime.Year * 365 + currentTime.DayOfYear;
//            }
//        }

//        private bool _initialized;
//        private const int FacebookPostCooldown = 5;
//    }
//}
