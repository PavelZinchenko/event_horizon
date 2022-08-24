using GameServices.GameManager;
using Services.Account;
using Services.Messenger;
using UnityEngine;
using Zenject;

namespace Gui.MainMenu
{
    public class SettingsPurchases : MonoBehaviour
    {
        [Inject] private readonly IGameDataManager _gameDataManager;
        [Inject] private readonly IAccount _account;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<Status>(EventType.AccountStatusChanged, OnAccountStatusChanged);
        }

        public void RestorePurchases()
        {
            if (_account.Status != Status.Connected)
            {
                _account.SignIn();
                _restoreOnNextLogin = true;
            }
            else
            {
                _gameDataManager.RestorePurchases();
            }
        }

        private void OnAccountStatusChanged(Status status)
        {
            if (status == Status.Connected && _restoreOnNextLogin)
                _gameDataManager.RestorePurchases();
            _restoreOnNextLogin = false;
        }

        private bool _restoreOnNextLogin = false;
    }
}
