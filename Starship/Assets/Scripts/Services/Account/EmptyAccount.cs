using System;
using UnityEngine;

namespace Services.Account
{
    public class EmptyAccount : IAccount
    {
        public void SignIn() { }
        public void SignOut() { }
        public bool CanSignOut => false;
        public bool CanSignIn => false;

        public Status Status { get { return Status.NotConnected; } }
        public string DisplayName { get { return string.Empty; } }
        public string Id { get { return string.Empty; } }
        public IObservable<Texture2D> LoadUserIcon() { return UniRx.Observable.Empty<Texture2D>(); }
    }
}
