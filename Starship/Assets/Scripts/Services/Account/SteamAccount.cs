#if UNITY_STANDALONE

using System;
using GameServices.Settings;
using Steamworks;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services.Account
{
    public class SteamAccount : IAccount
    {
        [Inject]
        public SteamAccount(GameSettings gameSettings, AccountStatusChangedSignal.Trigger accountChangedTrigger)
        {
            _gameSettings = gameSettings;
            _accountChangedTrigger = accountChangedTrigger;
        }

        public void SignIn() {}
        public void SignOut() {}
        public bool CanSignOut => false;
        public bool CanSignIn => false;

        public Status Status => SteamManager.Initialized ? Status.Connected : Status.NotConnected;

        public string DisplayName => SteamManager.Initialized ? SteamFriends.GetPersonaName() : string.Empty;
        public string Id => SteamManager.Initialized ? "s:" + SteamUser.GetSteamID() : string.Empty;

        public IObservable<Texture2D> LoadUserIcon()
        {
            return Observable.Empty<Texture2D>();
        }

        private readonly GameSettings _gameSettings;
        private readonly AccountStatusChangedSignal.Trigger _accountChangedTrigger;
    }
}

#endif