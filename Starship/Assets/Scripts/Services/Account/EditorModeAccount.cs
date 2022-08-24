using System;
using GameDatabase.Model;
using Services.Reources;
using UniRx;
using UnityEngine;
using Zenject;

namespace Services.Account
{
    public class EditorModeAccount : IAccount
    {
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly AccountStatusChangedSignal.Trigger _accountStatusChangedTrigger;

        public void SignIn()
        {
            Status = Status.Connecting;
            Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(value => Status = Status.Connected);
        }

        public void SignOut()
        {
            Status = Status.NotConnected;
        }

        public bool CanSignOut => true;
        public bool CanSignIn => true;

        public Status Status
        {
            get { return _status; }
            set
            {
                if (_status == value)
                    return;

                _status = value;
                _accountStatusChangedTrigger.Fire(_status);
            }
        }

        public string DisplayName { get { return "12345"; } }
        public string Id { get { return "PavelZinchenko2"; } }

        public System.IObservable<Texture2D> LoadUserIcon()
        {
            var subject = new Subject<Texture2D>();

            Observable.Timer(System.TimeSpan.FromSeconds(1)).Subscribe(value =>
            {
                var sprite = _resourceLocator.GetSprite(new SpriteId("M_07", SpriteId.Type.AvatarIcon));
                subject.OnNext(sprite.texture);
                subject.OnCompleted();
            });

            return subject;
        }

        private Status _status = Status.NotConnected;
    }
}
