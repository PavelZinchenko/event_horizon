using Constructor.Ships;
using Economy;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using Services.Localization;
using UnityEngine;
using Services.Reources;
using UnityEngine.UI;
using ViewModel.Common;
using Zenject;

namespace Gui.ShipService
{
    public class UpgradePanel : MonoBehaviour
    {
        [SerializeField] private ShipLayoutPanel _shipLayout;
        [SerializeField] private ToggleGroup _cellToggleGroup;

        [SerializeField] private PricePanel _price1;
        [SerializeField] private PricePanel _price2;
        [SerializeField] private PricePanel _price3;

        [SerializeField] private Text _warningText;
        [SerializeField] private Text _nothingSelectedText;
        [SerializeField] private Text _selectCellTypeText;
        [SerializeField] private Button _addButton;
        [SerializeField] private Button _resetButton;
        [SerializeField] private Toggle _outerBlockToggle;
        [SerializeField] private Toggle _innerBlockToggle;
        [SerializeField] private Toggle _engineBlockToggle;
        [SerializeField] private Toggle _weaponBlockToggle;

        [SerializeField] private AudioClip _buySound;

        [Inject] private readonly GuiHelper _guiHelper;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly PlayerInventory _playerInventory;
        [Inject] private readonly IResourceLocator _resourceLocator;
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        
        public void Initialize(IShip ship, Faction faction, int level)
        {
            _ship = ship;
            _faction = faction;
            _level = level;
            _selectedBlockX = -1;
            _selectedBlockY = -1;
            _shipLayout.ClearSelection();
            _shipInfo = new ShipInformation(_ship, _faction, _level);

            UpdateControls();
        }

        public void OnBlockSelected(int x, int y)
        {
            _selectedBlockX = x;
            _selectedBlockY = y;
            _outerBlockToggle.isOn = true;
            _innerBlockToggle.isOn = false;
            _engineBlockToggle.isOn = false;
            _weaponBlockToggle.isOn = false;
            UpdateControls();
        }

        public void OnCellTypeSelected(Toggle toggle)
        {
            if (toggle == _outerBlockToggle)
                _selectedCellType = CellType.Outer;
            else if (toggle == _innerBlockToggle)
                _selectedCellType = CellType.Inner;
            else if (toggle == _engineBlockToggle)
                _selectedCellType = CellType.Engine;
            else if (toggle == _weaponBlockToggle)
                _selectedCellType = CellType.Weapon;
        }

        public void OnAddButtonClicked()
        {
            var isBlockSelected = _selectedBlockX >= 0 && _selectedBlockY >= 0;
            if (!isBlockSelected || !_shipInfo.IsShipLevelEnough || !_shipInfo.IsShipyardLevelEnough || 
                !_shipInfo.Price1.IsEnough(_playerResources) || !_shipInfo.Price2.IsEnough(_playerResources)) return;
            if (!_ship.Model.LayoutModifications.TryAddCell(_selectedBlockX, _selectedBlockY, _selectedCellType)) return;

            _shipInfo.Price1.Withdraw(_playerResources);
            _shipInfo.Price2.Withdraw(_playerResources);

            _soundPlayer.Play(_buySound);
            _selectedBlockX = _selectedBlockY = -1;
            _shipInfo = new ShipInformation(_ship, _faction, _level);
            _shipLayout.Initialize(_ship.Model.Layout);
            UpdateControls();
        }

        public void OnResetButtonClicked()
        {
            _guiHelper.ShowConfirmation(_localization.GetString("$RemoveShipCellsWarning"), ResetLayout);
        }

        public void ResetLayout()
        {
            if (!_shipInfo.CanReset || !_shipInfo.ResetPrice.IsEnough(_playerResources)) return;

            _ship.Model.LayoutModifications.Reset();
            _ship.RemoveInvalidComponents(_playerInventory.Components);

            _shipInfo.ResetPrice.Withdraw(_playerResources);
            _soundPlayer.Play(_buySound);
            _selectedBlockX = _selectedBlockY = -1;
            _shipInfo = new ShipInformation(_ship, _faction, _level);
            _shipLayout.Initialize(_ship.Model.Layout);
            UpdateControls();
        }

        private void UpdateControls()
        {
            var isBlockSelected = _selectedBlockX >= 0 && _selectedBlockY >= 0;

            if (!_shipInfo.IsShipLevelEnough)
            {
                _warningText.gameObject.SetActive(true);
                _warningText.text = _localization.GetString("$LowLevelText", _shipInfo.RequiredLevel.ToString());
            }
            else if (!_shipInfo.IsShipyardLevelEnough)
            {
                _warningText.gameObject.SetActive(true);
                _warningText.text = _localization.GetString("$RequiredShipyardLevel", _shipInfo.RequiredShipyardLevel, _localization.GetString(_ship.Model.Faction.Name));
            }
            else
            {
                _warningText.gameObject.SetActive(false);
            }

            _nothingSelectedText.gameObject.SetActive(!isBlockSelected && _shipInfo.IsShipLevelEnough);
            _selectCellTypeText.gameObject.SetActive(isBlockSelected);

            _cellToggleGroup.gameObject.SetActive(isBlockSelected);

            if (isBlockSelected)
            {
                var price1 = _shipInfo.Price1;
                var price2 = _shipInfo.Price2;

                _price1.Initialize(price1, price1.IsEnough(_playerResources));
                _price2.Initialize(price2, price2.IsEnough(_playerResources));
                _price2.gameObject.SetActive(price2.Amount > 0);

                _addButton.interactable = _shipInfo.IsShipLevelEnough && _shipInfo.IsShipyardLevelEnough && price1.IsEnough(_playerResources) && price2.IsEnough(_playerResources);

                _outerBlockToggle.interactable = _ship.Model.LayoutModifications.IsCellValid(_selectedBlockX, _selectedBlockY, CellType.Outer);
                _innerBlockToggle.interactable = _ship.Model.LayoutModifications.IsCellValid(_selectedBlockX, _selectedBlockY, CellType.Inner);
                _engineBlockToggle.interactable = _ship.Model.LayoutModifications.IsCellValid(_selectedBlockX, _selectedBlockY, CellType.Engine);
                _weaponBlockToggle.interactable = _ship.Model.LayoutModifications.IsCellValid(_selectedBlockX, _selectedBlockY, CellType.Weapon);
            }
            else
            {
                _price1.Initialize(Currency.Credits);
                _price2.Initialize(Currency.Stars);
                _price2.gameObject.SetActive(CurrencyExtensions.PremiumCurrencyAllowed);
                _addButton.interactable = false;
            }

            if (_shipInfo.CanReset)
                _price3.Initialize(_shipInfo.ResetPrice, _shipInfo.ResetPrice.IsEnough(_playerResources));
            else
                _price3.Initialize(CurrencyExtensions.PremiumCurrencyAllowed ? Currency.Stars : Currency.Credits);

            _resetButton.interactable = _shipInfo.CanReset && _shipInfo.ResetPrice.IsEnough(_playerResources);
        }

        private CellType _selectedCellType = CellType.Outer;
        private int _selectedBlockX = -1;
        private int _selectedBlockY = -1;
        private IShip _ship;
        private int _level;
        private Faction _faction;
        private ShipInformation _shipInfo;

        private struct ShipInformation
        {
            public ShipInformation(IShip ship, Faction shipyardFaction, int shipyardLevel)
            {
                _shipLevel = ship.Experience.Level;

                TotalCells = ship.Model.LayoutModifications.TotalExtraCells();
                Cells = ship.Model.LayoutModifications.ExtraCells();

                var starsAllowed = CurrencyExtensions.PremiumCurrencyAllowed;

                Price1 = Price.Common(starsAllowed ? (Cells + 1) * 1000 : (Cells+1)*2000);
                Price2 = starsAllowed ? Price.Premium(1 + Cells/2) : new Price(0, Currency.Credits);
                ResetPrice = starsAllowed ? Price.Premium(Cells) : Price.Common(Cells*2000);

                RequiredLevel = TotalCells > 0 ? 5 + Mathf.Min(5 * Cells, 95 * Cells / TotalCells) : 0;

                RequiredShipyardLevel = Cells * 5 + 5;
                IsShipyardLevelEnough = shipyardLevel >= RequiredShipyardLevel &&
                    (ship.Model.Faction == Faction.Neutral || ship.Model.Faction == shipyardFaction);
            }

            public bool CanReset { get { return Cells > 0; } }

            public bool IsShipLevelEnough { get { return _shipLevel >= RequiredLevel; } }

            public readonly bool IsShipyardLevelEnough;
            public readonly int RequiredShipyardLevel;
            public readonly int TotalCells;
            public readonly int Cells;
            public readonly Price Price1;
            public readonly Price Price2;
            public readonly Price ResetPrice;
            public readonly int RequiredLevel;

            private readonly int _shipLevel;
        }
    }
}
