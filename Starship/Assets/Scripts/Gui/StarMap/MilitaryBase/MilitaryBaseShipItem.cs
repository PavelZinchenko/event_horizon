using Constructor.Ships;
using Economy;
using GameServices.Player;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using ViewModel.Common;
using Zenject;

namespace Gui.StarMap
{
    public class MilitaryBaseShipItem : MonoBehaviour
    {
        [SerializeField] private Image _icon;
        [SerializeField] private Text _levelText;
        [SerializeField] private Text _levelTextAlt;
        [SerializeField] private Text _nameText;
        [SerializeField] private Text _classText;
        [SerializeField] private PricePanel _pricePanel;
        [SerializeField] private Button _upgradeButton;
        [SerializeField] private Slider _expSlider;
        [SerializeField] private Text _expText;
        [SerializeField] private Text _requiredExpText;
        [SerializeField] private GameObject _maxLevelReachedObject;

        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly PlayerResources _resources;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public void Initialize(IShip ship, Price price, int maxLevel)
        {
            Ship = ship;
            _nameText.text = _localization.GetString(ship.Name);
            _nameText.color = ColorTable.QualityColor(ship.Model.Quality());
            _levelTextAlt.text = _levelText.text = ship.Experience.Level.ToString();
            _classText.text = ship.Model.SizeClass.ToString(_localization);

            var exp = ship.Experience.ExpFromLastLevel;
            var required = ship.Experience.NextLevelCost;
            _expText.text = exp.ToString();
            _requiredExpText.text = required.ToString();
            _expSlider.value = (float)exp / (float)required;

            _maxLevelReachedObject.SetActive(ship.Experience.Level >= maxLevel);

            _icon.sprite = _resourceLocator.GetSprite(ship.Model.ModelImage);
            _pricePanel.Initialize(null, price, !price.IsEnough(_resources));
            _upgradeButton.interactable = ship.Experience.Level < maxLevel && price.IsEnough(_resources);
        }

        public IShip Ship { get; private set; }
    }
}