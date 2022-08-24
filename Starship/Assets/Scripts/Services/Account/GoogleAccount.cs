#if UNITY_ANDROID

using GameServices.Settings;
using UniRx;
using UnityEngine;
using Zenject;
using GooglePlayGames;

namespace Services.Account
{
    class GoogleAccount : IAccount
    {
        [Inject]
        public GoogleAccount(GameSettings gameSettings, AccountStatusChangedSignal.Trigger accountChangedTrigger)
        {
            _gameSettings = gameSettings;
            _accountChangedTrigger = accountChangedTrigger;
        }

        public Status Status 
        {
            get
            {
                if (_status == Status.Connected)
                    if (!PlayGamesPlatform.Instance.IsAuthenticated())
                        Status = Status.NotConnected;

                return _status;
            }
            private set
            {
                if (_status == value)
                    return;
                _status = value;

                UnityEngine.Debug.Log("GoogleAccount.Status = " + value);
                _accountChangedTrigger.Fire(_status);
            }
        }

        public string DisplayName { get { return PlayGamesPlatform.Instance.GetUserDisplayName(); } }
        public string Id { get { return "G:" + PlayGamesPlatform.Instance.GetUserId(); } }

        public System.IObservable<Texture2D> LoadUserIcon()
        {
            if (Status != Status.Connected)
                return Observable.Empty<Texture2D>();

            var url = PlayGamesPlatform.Instance.GetUserImageUrl();
            if (string.IsNullOrEmpty(url))
                return Observable.Empty<Texture2D>();

            return ObservableWWW.GetWWW(url).Select(www => www.texture);
        }

        public void SignIn()
        {
            UnityEngine.Debug.Log("GoogleAccount.SignIn()");

            _gameSettings.SignedIn = false;
            Status = Status.Connecting;

            try
            {
                PlayGamesPlatform.Instance.Authenticate((bool success) => 
                {
                    UnityEngine.Debug.Log("SignIn: " + success);
                    _gameSettings.SignedIn = success;
                    Status = success ? Status.Connected : Status.FailedToConnect;
                });
            }
            catch (System.Exception)
            {
                Status = Status.FailedToConnect;
                _gameSettings.SignedIn = false;
            }
        }

        public void SignOut()
        {
            _gameSettings.SignedIn = false;
            PlayGamesPlatform.Instance.SignOut();
            Status = Status.NotConnected;
        }

        public bool CanSignOut => true;
        public bool CanSignIn => true;

        private Status _status = Status.NotConnected;
        private readonly GameSettings _gameSettings;
        private readonly AccountStatusChangedSignal.Trigger _accountChangedTrigger;
    }
}

#endif