using Services.Account;
using Services.Gui;
using UnityEngine;
using Zenject;

namespace Gui.Common
{
    public class SignInWindow : MonoBehaviour
    {
        [Inject]
        private void Initialize(AccountStatusChangedSignal accountStatusChangedSignal)
        {
            _accountStatusChangedSignal = accountStatusChangedSignal;
            _accountStatusChangedSignal.Event += OnAccountStatusChanged;
        }

        private void OnAccountStatusChanged(Services.Account.Status status)
        {
            var window = GetComponent<IWindow>();

            if (status == Services.Account.Status.Connecting)
                window.Open();
            else
                window.Close();
        }

        private AccountStatusChangedSignal _accountStatusChangedSignal;
    }
}
