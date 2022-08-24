using System;
using GameServices.Settings;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services.Account
{
    public class GameCenterAccount : IAccount, IInitializable, IDisposable
    {
        [Inject]
        public GameCenterAccount(GameSettings gameSettings, AccountStatusChangedSignal.Trigger accountChangedTrigger)
        {
            _gameSettings = gameSettings;
            _accountChangedTrigger = accountChangedTrigger;

            _status = GameCenterManager.IsInitialized && GameCenterManager.IsPlayerAuthenticated ? Status.Connected : Status.NotConnected;
        }

        public void Initialize()
        {
            GameCenterManager.OnAuthFinished += OnAuthFinished;
        }

        public void Dispose()
        {
            GameCenterManager.OnAuthFinished -= OnAuthFinished;
        }

        public void SignIn()
        {
            UnityEngine.Debug.Log("GameCenterAccount.Signin");

            if (!GameCenterManager.IsInitialized)
                GameCenterManager.Init();
        }

        public void SignOut()
        {
        }

        public bool CanSignOut => false;
        public bool CanSignIn => true;

        public Status Status
        {
            get
            {
                return _status;
            }
            private set
            {
                if (_status == value)
                    return;
                _status = value;

                UnityEngine.Debug.Log("GameCenterAccount.Status = " + value);
                _accountChangedTrigger.Fire(_status);
            }
        }

        private void OnAuthFinished(SA.Common.Models.Result result)
        {
            UnityEngine.Debug.Log("GameCenterAccount.OnAuthFinished: " + (result.HasError ? result.Error.Message : "succeeded"));
            Status = result.IsSucceeded ? Status.Connected : Status.FailedToConnect;
            _gameSettings.SignedIn = result.IsSucceeded;

            if (result.IsSucceeded)
                GameCenterManager.Player.OnPlayerPhotoLoaded += OnPlayerPhotoLoaded;
        }

        public string DisplayName { get { return GameCenterManager.Player.Alias; } }
        public string Id { get { return "i:" + GameCenterManager.Player.Id; } }

        public IObservable<Texture2D> LoadUserIcon()
        {
            if (!GameCenterManager.IsInitialized)
                return Observable.Empty<Texture2D>();

            var subject = _subject = new Subject<Texture2D>();
            GameCenterManager.Player.LoadPhoto(GK_PhotoSize.GKPhotoSizeSmall);

            return subject;
        }

        private static void OnPlayerPhotoLoaded(GK_UserPhotoLoadResult result)
        {
            if (_subject == null) return;

            if (result.Photo != null)
            {
                _subject.OnNext(result.Photo);
                _subject.OnCompleted();
            }
            else
            {
                _subject.OnError(new NullReferenceException());
            }

            _subject = null;
        }

        private static Subject<Texture2D> _subject;
        private Status _status = Status.NotConnected;
        private readonly GameSettings _gameSettings;
        private readonly AccountStatusChangedSignal.Trigger _accountChangedTrigger;
    }
}
