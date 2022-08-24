using UnityEngine;
using UnityEngine.UI;
using Constructor.Ships;
using Economy;
using GameDatabase;
using GameServices.Player;
using Services.Audio;
using Services.Localization;
using Services.Messenger;
using Services.ObjectPool;
using Zenject;

namespace Gui.StarMap
{
    public class MilitaryBase : MonoBehaviour
    {
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly GameObjectFactory _gameObjectFactory;
        [Inject] private readonly PlayerFleet _playerFleet;

        [Inject]
        private void Initialize(IMessenger _messenger)
        {
            _messenger.AddListener<int>(EventType.MoneyValueChanged, value => UpdateResources());
            _messenger.AddListener<int>(EventType.StarsValueChanged, value => UpdateResources());
        }

        [SerializeField] private LayoutGroup _shipList;
        [SerializeField] private Image _factionIcon;
        [SerializeField] private Text _factionText;
        [SerializeField] private Text _levelText;
        [SerializeField] private AudioClip _buySound;
        [SerializeField] private Text _creditsText;
        [SerializeField] private Text _starsText;
        [SerializeField] private GameObject _starsPanel;

        public void InitializeWindow()
        {
            var faction = _motherShip.CurrentStar.Region.Faction;
            _factionText.text = _localization.GetString("$MilitaryBase", _localization.GetString(faction.Name));
            _factionIcon.color = faction.Color;
            _levelText.text = Level.ToString();

            UpdateContent();
            UpdateResources();
        }

        public void LevelUpButtonClicked(MilitaryBaseShipItem shipItem)
        {
            var ship = shipItem.Ship;
            if (ship.Experience.Level >= Level || ship.Experience >= Maths.Experience.MaxPlayerExperience)
                return;

            var price = GetLevelUpPrice(ship);
            if (!price.TryWithdraw(_playerResources))
                return;

            ship.Experience = System.Math.Min((long)Maths.Experience.MaxPlayerExperience, (long)ship.Experience + ship.Experience.NextLevelCost);
            _soundPlayer.Play(_buySound);

            UpdateContent();
        }

        private void UpdateContent()
        {
            _shipList.InitializeElements<MilitaryBaseShipItem, IShip>(_playerFleet.ActiveShipGroup.Ships, UpdateShipItem, _gameObjectFactory);
        }

        private void UpdateShipItem(MilitaryBaseShipItem item, IShip ship)
        {
            item.Initialize(ship, GetLevelUpPrice(ship), Mathf.Min(Level, Maths.Experience.MaxPlayerRank));
        }

        private void UpdateResources()
        {
            _creditsText.text = _playerResources.Money.ToString();
            _starsText.text = _playerResources.Stars.ToString();
            _starsPanel.SetActive(Economy.CurrencyExtensions.PremiumCurrencyAllowed);
        }

        private int Level { get { return Mathf.Max(5, _motherShip.CurrentStar.Level/2); } }

        private static Price GetLevelUpPrice(IShip ship)
        {
            var size = 1 + Mathf.Max(0, (int)ship.Model.SizeClass);
            var price = 1 + ship.Experience.Level * size / 25;
            return Price.Premium(price);
        }
    }
}
