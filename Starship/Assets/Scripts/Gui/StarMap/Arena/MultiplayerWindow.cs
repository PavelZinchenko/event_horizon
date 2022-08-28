using Economy;
using GameModel.Quests;
using GameServices.Multiplayer;
using GameServices.Player;
using Gui.Windows;
using Services.Account;
using Services.Audio;
using UnityEngine;
using Services.Gui;
using Services.Localization;
using Services.Messenger;
using UnityEngine.UI;
using ViewModel;
using Zenject;
using Status = GameServices.Multiplayer.Status;

namespace Gui.Multiplayer
{
    public class MultiplayerWindow : MonoBehaviour
    {
        [SerializeField] private ShipListContentFiller _shipListContentFiller;
        [SerializeField] private ListScrollRect _shipListScrollRect;
        [SerializeField] private ArenaFightDialog _fightDialog;
        [SerializeField] private GameObject _waiter;
        [SerializeField] private Text _playerName;
        [SerializeField] private Text _rating;
        [SerializeField] private Text _availableBattlesText;
        [SerializeField] private TimerViewModel _timer;

        [SerializeField] private GameObject _timerPanel;
        [SerializeField] private GameObject _errorButton;
        [SerializeField] private GameObject _signInButton;
        [SerializeField] private GameObject _notAllowedButton;
        [SerializeField] private GameObject _trainingButton;
        [SerializeField] private GameObject _shopButton;
        [SerializeField] private GameObject _findOpponentButton;
        [SerializeField] private GameObject _buyButton;
        [SerializeField] private GameObject _adItem;

        [SerializeField] private Button _showAdButton;
        [SerializeField] private Button _refreshAdButton;
        [SerializeField] private Text _adNotAvailableText;
        [SerializeField] private Text _watchAdText;
        [SerializeField] private GameObject _adWaiter;

        [SerializeField] private AudioClip _buySound;
        [SerializeField] private AudioClip _fightSound;

        [SerializeField] private AnimatedWindow _storeWindow;

        [SerializeField] private Image _factionIcon;
        [SerializeField] private Text _factionText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _creditsText;
        [SerializeField] private Text _starsText;
        [SerializeField] private Text _tokensText;
        [SerializeField] private GameObject _starsPanel;

        [Inject] private readonly OfflineMultiplayer _multiplayer;
        [Inject] private readonly IAccount _account;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly InventoryFactory _inventoryFactory;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<Status>(EventType.MultiplayerStatusChanged, OnStatusChanged);
            messenger.AddListener<IPlayerInfo>(EventType.EnemyFleetLoaded, OnEnemyFleetReady);
            messenger.AddListener<IPlayerInfo>(EventType.ArenaEnemyFound, OnEnemyFound);
            messenger.AddListener<int>(EventType.MoneyValueChanged, value => UpdateResources());
            messenger.AddListener<int>(EventType.StarsValueChanged, value => UpdateResources());
            messenger.AddListener<int>(EventType.TokensValueChanged, value => UpdateResources());
        }

        public void Initialize(WindowArgs args)
        {
            var faction = _motherShip.CurrentStar.Region.Faction;
            _factionText.text = _localization.GetString("$ArenaPanelHeader"/*, _localization.GetString(faction.Name)*/);
            _factionIcon.color = faction.Color;
            _levelText.text = _motherShip.CurrentStar.Level.ToString();
            UpdateResources();

            _shipListContentFiller.Initialize(_multiplayer.Player.Fleet);
            _shipListScrollRect.RefreshContent();

            UpdateStatus();
            OnStatusChanged(_multiplayer.Status);
        }

        public void SignIn()
        {
            _account.SignIn();
        }

        public void UpdateStatus()
        {
            _multiplayer.UpdateStatus();
        }

        public void FindOpponent()
        {
            _multiplayer.FindOpponent();
        }

        public void StartTraining()
        {
            _multiplayer.PrepareToFight(_multiplayer.Player);
        }

        public void BuyMore()
        {
            if (_playerResources.Stars > 10)
            {
                _playerResources.Stars -= 10;
                _multiplayer.BuyMoreTickets();
                OnStatusChanged(_multiplayer.Status);
                _soundPlayer.Play(_buySound);
            }
        }

        public void OpenStore()
        {
            _storeWindow.Open(new WindowArgs(_inventoryFactory.CreateArenaInventory(_motherShip.CurrentStar)));
        }

        private void OnEnemyFound(IPlayerInfo enemy)
        {
            if (!gameObject.activeSelf) return;
            _multiplayer.PrepareToFight(enemy);
        }

        private void OnEnemyFleetReady(IPlayerInfo enemy)
        {
            if (!gameObject.activeSelf) return;
            _soundPlayer.Play(_fightSound);
            _fightDialog.Initialize(enemy);
        }

        private void OnStatusChanged(Status status)
        {
            var loading = status == Status.Connecting;
            var ready = status == Status.Ready;
            var availableBattles = _multiplayer.AvailableBattles;

            _waiter.SetActive(loading);
            _timerPanel.SetActive(ready && CurrencyExtensions.PremiumCurrencyAllowed);
            _errorButton.SetActive(status == Status.ConnectionError || status == Status.Unknown);
            _signInButton.SetActive(status == Status.NotLoggedIn);
            _notAllowedButton.SetActive(status == Status.ShipNotAllowed);
            _trainingButton.SetActive(!loading);
            _shopButton.SetActive(!loading);
            _findOpponentButton.SetActive(status == Status.Ready && availableBattles > 0);
            _buyButton.SetActive(Economy.CurrencyExtensions.PremiumCurrencyAllowed && status == Status.Ready && availableBattles == 0);
            _trainingButton.SetActive(ready && availableBattles > 0);

            UpdatePlayer();
        }

        private void Update()
        {
            var time = _multiplayer.TimeToNextBattle;
            _timer.gameObject.SetActive(time > 0);
            _timer.SetTime(time);

            if (_availableBattles != _multiplayer.AvailableBattles)
            {
                _availableBattles = _multiplayer.AvailableBattles;
                _availableBattlesText.text = _localization.GetString("$ArenaBattlesAvailable", _availableBattles.ToString());
            }
        }

        private void UpdateResources()
        {
            _creditsText.text = _playerResources.Money.ToString();
            _tokensText.text = _playerResources.Tokens.ToString();
            _starsText.text = _playerResources.Stars.ToString();
            _starsPanel.SetActive(Economy.CurrencyExtensions.PremiumCurrencyAllowed);
        }

        private void UpdatePlayer()
        {
            var name = _multiplayer.Player.Name;
            _playerName.text = string.IsNullOrEmpty(name) ? Unknown : name;
            _rating.text = _multiplayer.Player.Rating.ToString();
        }

        private int _availableBattles = -1;
        private const string Unknown = "???";
    }
}
