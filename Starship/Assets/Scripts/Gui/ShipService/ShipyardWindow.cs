using System.Collections;
using Constructor.Ships;
using GameDatabase.DataModel;
using GameServices.Player;
using GameServices.Research;
using Gui.Craft;
using UnityEngine;
using Services.Gui;
using Services.Localization;
using Services.Messenger;
using Services.Reources;
using Services.Unity;
using UnityEngine.UI;
using ViewModel;
using Zenject;

namespace Gui.ShipService
{
    public class ShipyardWindow : MonoBehaviour
    {
        [SerializeField] private FleetPanel _fleetPanel;
        [SerializeField] private UpgradePanel _upgradeLayoutPanel;
        [SerializeField] private PaintingPanel _paintingPanel;
        [SerializeField] private ModificationsPanel _modificationsPanel;
        [SerializeField] private GameObject _modificationsRightPanel;

        [SerializeField] private GameObject _shipLayoutPanel;
        [SerializeField] private GameObject _shipLayoutContainer;
        [SerializeField] private ShipLayoutPanel _shipLayout;
        [SerializeField] private Zoom _zoom;

        [SerializeField] private Image _factionIcon;
        [SerializeField] private Text _factionText;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _creditsText;
        [SerializeField] private Text _starsText;
        [SerializeField] private Image _techIcon;
        [SerializeField] private Text _techText;

        [Inject] private readonly MotherShip _motherShip;
        [Inject] private readonly Research _research;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly ICoroutineManager _coroutineManager;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;

        [Inject]
        private void Initialize(IMessenger messenger)
        {
            messenger.AddListener<int>(EventType.MoneyValueChanged, value => UpdateResources());
            messenger.AddListener<int>(EventType.StarsValueChanged, value => UpdateResources());
            messenger.AddListener(EventType.TechPointsChanged, UpdateResources);
        }

        public void Initialize(WindowArgs args)
        {
            if (args.Count >= 2)
            {
                _faction = args.Get<Faction>(0);
                _level = args.Get<int>(1);
            }
            else
            {
                var star = _motherShip.CurrentStar;
                _faction = star.Region.Faction;
                _level = Mathf.Max(5, star.Level);
            }

            var color = _faction.Color;
            _factionText.text = _localization.GetString(_faction.Name);
            _factionIcon.color = color;
            _techIcon.color = color;
            _levelText.text = _level.ToString();

            UpdateResources();
            ShowShipList();
        }

        public void ShowShipList()
        {
            _paintingPanel.gameObject.SetActive(false);
            _upgradeLayoutPanel.gameObject.SetActive(false);
            _fleetPanel.gameObject.SetActive(true);
            _modificationsPanel.gameObject.SetActive(false);
            _modificationsRightPanel.SetActive(false);
            ShowLayout(false);
            _fleetPanel.Initialize();
        }

        public void ShowUpgradePanel()
        {
            if (_ship == null) return;

            _fleetPanel.gameObject.SetActive(false);
            _paintingPanel.gameObject.SetActive(false);
            _upgradeLayoutPanel.gameObject.SetActive(false);
            _modificationsRightPanel.SetActive(true);
            _modificationsPanel.gameObject.SetActive(true);
            ShowLayout(false);
            _modificationsPanel.Initialize(_ship, _level, _faction);
        }

        public void ShowLayoutPanel()
        {
            if (_ship == null) return;

            _fleetPanel.gameObject.SetActive(false);
            _paintingPanel.gameObject.SetActive(false);
            _upgradeLayoutPanel.gameObject.SetActive(true);
            _modificationsPanel.gameObject.SetActive(false);
            _modificationsRightPanel.SetActive(false);
            ShowLayout(true);
            _upgradeLayoutPanel.Initialize(_ship, _faction, _level);
        }

        public void ShowPaintingPanel()
        {
            if (_ship == null) return;

            _fleetPanel.gameObject.SetActive(false);
            _upgradeLayoutPanel.gameObject.SetActive(false);
            _paintingPanel.gameObject.SetActive(true);
            _modificationsPanel.gameObject.SetActive(false);
            _modificationsRightPanel.SetActive(false);
            ShowLayout(false);
            _paintingPanel.Initialize(_ship);
        }

        public void OnShipSelected(IShip ship)
        {
            _ship = ship;
            ShowLayoutPanel();
        }

        private void ShowLayout(bool visible)
        {
            _shipLayoutPanel.gameObject.SetActive(visible);
            if (!visible) return;

            _shipLayout.Initialize(_ship.Model.Layout);
            _shipLayout.BackgroundImage.sprite = _resourceLocator.GetSprite(_ship.Model.ModelImage);
            _coroutineManager.StartCoroutine(CalculateZoom());
        }

        private IEnumerator CalculateZoom()
        {
            yield return null;
            _zoom.Value = CalculateZoomFactor();
        }

        private float CalculateZoomFactor()
        {
            var transform = _shipLayoutContainer.GetComponent<RectTransform>();
            var size = transform.rect.size;
            var container = transform.parent.GetComponent<RectTransform>().rect.size;
            return Mathf.Clamp01(container.y / size.y);
        }

        private void UpdateResources()
        {
            if (!gameObject.activeSelf)
                return;

            _creditsText.text = _playerResources.Money.ToString();
            _starsText.text = _playerResources.Stars.ToString();
            _techText.text = _research.GetAvailablePoints(_motherShip.CurrentStar.Region.Faction).ToString();
        }

        private IShip _ship;
        private Faction _faction;
        private int _level;
    }
}
