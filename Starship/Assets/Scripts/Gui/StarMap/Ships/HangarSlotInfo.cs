using GameStateMachine.States;
using Services.Localization;
using Constructor.Ships;
using Economy.ItemType;
using Economy.Products;
using GameServices.Gui;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    public class HangarSlotInfo : MonoBehaviour
    {
        [Inject] private readonly OpenConstructorSignal.Trigger _openConstructorTrigger;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly GuiHelper _guiHelper;

        [SerializeField] private GameObject _iconPanel;
        [SerializeField] private Image _lockIcon;
        [SerializeField] private Image _emptyIcon;
        [SerializeField] private Image _shipIcon;
        [SerializeField] private Button _constructorButton;
        [SerializeField] private Button _removeButton;
        [SerializeField] private Button _installButton;
        [SerializeField] private Text _shipNameText;
        [SerializeField] private Text _shipLevelText;
        //[SerializeField] private Text _shipClassText;
        [SerializeField] private Text _emptySlotText;
        [SerializeField] private GameObject _lockedLabel;
        [SerializeField] private GameObject _levelPanel;
        //[SerializeField] private GameObject _classPanel;
        [SerializeField] private Slider _experienceSlider;
        [SerializeField] private Text _expText;
        [SerializeField] private Text _requiredExpText;
        [SerializeField] private ModsPanel _modsPanel;

        public void OnConstructorButtonClicked()
        {
            if (_ship != null)
                _openConstructorTrigger.Fire(_ship);
        }

        public void Initialize(IShip ship, bool canInstall)
        {
            _ship = ship;
            InitializeShipInfo(ship);

            _lockIcon.gameObject.SetActive(false);
            _lockedLabel.gameObject.SetActive(false);
            _emptyIcon.gameObject.SetActive(false);
            _emptySlotText.gameObject.SetActive(false);
            _removeButton.gameObject.SetActive(false);
            _installButton.gameObject.SetActive(true);
            _installButton.interactable = canInstall;
        }

        public void Initialize(HangarItem item)
        {
            _ship = item.Ship;
            InitializeShipInfo(_ship);

            _lockIcon.gameObject.SetActive(item.Locked);
            _lockedLabel.gameObject.SetActive(item.Locked);
            _emptyIcon.gameObject.SetActive(item.Ship == null);
            _emptySlotText.gameObject.SetActive(_ship == null && !item.Locked);
            _removeButton.gameObject.SetActive(_ship != null);
            _installButton.gameObject.SetActive(false);

            if (item.Ship == null)
            {
                _emptySlotText.text = _localization.GetString("$EmptySlot" + (int)item.Size);
                _emptyIcon.sprite = item.EmptySprite;
            }
        }

        public void Clear()
        {
            _ship = null;
            _iconPanel.gameObject.SetActive(false);
            _constructorButton.gameObject.SetActive(false);
            _removeButton.gameObject.SetActive(false);
            _installButton.gameObject.SetActive(false);

            _lockedLabel.gameObject.SetActive(false);
            _shipNameText.gameObject.SetActive(false);
            _emptySlotText.gameObject.SetActive(false);
            _levelPanel.SetActive(false);
            //_classPanel.SetActive(false);
            _experienceSlider.gameObject.SetActive(false);
        }

        public void ShowInformation()
        {
            if (Ship != null)
                _guiHelper.ShowItemInfoWindow(Ship);
        }

        public IShip Ship { get { return _ship; } }

        private void InitializeShipInfo(IShip ship)
        {
            _iconPanel.gameObject.SetActive(true);
            _constructorButton.gameObject.SetActive(ship != null);
            _shipIcon.gameObject.SetActive(ship != null);
            _shipNameText.gameObject.SetActive(ship != null);
            _levelPanel.SetActive(ship != null);
            //_classPanel.SetActive(ship != null);
            _experienceSlider.gameObject.SetActive(ship != null);
            _modsPanel.gameObject.SetActive(ship != null);

            if (ship != null)
            {
                _shipIcon.sprite = _resourceLocator.GetSprite(ship.Model.ModelImage);
                _shipIcon.color = ship.ColorScheme.HsvColor;
                _shipNameText.text = _localization.GetString(ship.Name);
                _shipNameText.color = ColorTable.QualityColor(ship.Model.Quality());
                _shipLevelText.text = ship.Experience.Level.ToString();
                //_shipClassText.text = ship.Model.Info.SizeClass.ToString(_localization);
                _modsPanel.Initialize(ship.Model.Modifications, _resourceLocator);

                var exp = ship.Experience.ExpFromLastLevel;
                var required = ship.Experience.NextLevelCost;
                _expText.text = exp.ToString();
                _requiredExpText.text = required.ToString();
                _experienceSlider.value = (float)exp / (float)required;
            }
        }

        private IShip _ship;
    }
}
