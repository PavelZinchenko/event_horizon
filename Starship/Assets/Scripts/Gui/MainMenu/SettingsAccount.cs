using GameServices.GameManager;
using GameServices.Settings;
using Services.Account;
using Services.Messenger;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using UniRx;

namespace Gui.MainMenu
{
    public class SettingsAccount : MonoBehaviour
    {
        [SerializeField] RectTransform _signInButton;
        [SerializeField] RectTransform _signOutButton;
        [SerializeField] RawImage _accountIcon;
        [SerializeField] Text _accountName;
        [SerializeField] Image _notConnectedIcon;
        [SerializeField] Text _notConnectedText;
        [SerializeField] private GameObject[] _controls;

        [Inject] private readonly GameSettings _gameSettings;
        [Inject] private readonly IAccount _account;
        [Inject] private readonly IGameDataManager _gameDataManager;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<Status>(EventType.AccountStatusChanged, OnAccountChanged);
        }

        public void SignIn()
        {
            _account.SignIn();
        }

        public void SignOut()
        {
            _account.SignOut();
        }

        private void OnAccountChanged(Status status)
        {
            if (!gameObject.activeSelf)
                return;

            var connected = status == Status.Connected;
            _signInButton.gameObject.SetActive(!connected && _account.CanSignIn);
            _signOutButton.gameObject.SetActive(connected && _account.CanSignOut);

            _accountName.gameObject.SetActive(connected);
            _notConnectedText.gameObject.SetActive(!connected);

            _notConnectedIcon.gameObject.SetActive(!connected);
            _accountIcon.gameObject.SetActive(connected);

            if (connected)
                _accountName.text = _account.DisplayName;

            _account.LoadUserIcon().Subscribe(result => _accountIcon.texture = result);
        }

        private void OnEnable()
        {
            OnAccountChanged(_account.Status);
        }
    }
}
