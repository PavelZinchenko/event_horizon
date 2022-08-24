using System.Linq;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase.Enums;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using Services.Gui;
using Services.Localization;
using Services.Reources;
using ViewModel;
using Zenject;

namespace Gui.StarMap
{
    public class CargoHold : MonoBehaviour
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly PlayerResources _playerResources;
        [Inject] private readonly GuiHelper _helper;
        [Inject] private readonly InventoryFactory _inventoryFactory;
        [Inject] private readonly IResourceLocator _resourceLocator;

        public enum Filter
        {
            All = -1,
            Resource = 0,
            Ship = 1,
            Weapon = 2,
            Module = 3,
            Other = 4,
        }

        [SerializeField] private ToggleGroup ItemsGroup;
        [SerializeField] private AudioClip Sound;
        [SerializeField] private GameObject QuantityPanel;
        [SerializeField] private Slider QuantitySlider;
        [SerializeField] private RadioGroupViewModel FilterGroup;
        [SerializeField] private ViewModel.Common.PricePanel PricePanel;
        [SerializeField] private GameObject DescriptionPanel;
        [SerializeField] private Text QuantityText;
        [SerializeField] private Text NameText;
        [SerializeField] private Text DescrtiptionText;
        [SerializeField] private Image Icon;
        [SerializeField] private GameObject EmptyLabel;
        [SerializeField] private Text MoneyText;
        [SerializeField] private Text StarText;
        [SerializeField] private GameObject StarPanel;
        [SerializeField] private Button ScrapButton;

        [SerializeField] private MarketContentFiller ContentFiller;
        [SerializeField] private ListScrollRect ItemList;

        public void Initialize(WindowArgs args)
        {
            _inventory = _inventoryFactory.CreateCargoHoldInventory();
            DescriptionPanel.gameObject.SetActive(false);

            FilterGroup.Value = (int)Filter.All;
            EmptyLabel.gameObject.SetActive(UpdatePlayerItems(true) == 0);
            _selectedItem = null;
            ItemsGroup.SetAllTogglesOff();
            UpdateButtons();
            UpdateStats();
        }

        public void OnItemSelected(ViewModel.Common.InventoryItem item)
        {
            _selectedItem = item;
            _quantity = 1;
            ContentFiller.OnItemSelected(item);
            UpdateButtons();
        }

        public void OnItemDeselected()
        {
            _selectedItem = null;
            UpdateButtons();
        }

        public void MoreInfoButtonClicked()
        {
            if (_selectedItem != null)
                _helper.ShowItemInfoWindow(_selectedItem.Product);
        }

        public void ScrapButtonClicked()
        {
            if (!_selectedItem) return;

            _selectedItem.Product.Sell(_quantity);
            _soundPlayer.Play(Sound);
            UpdateItems();
        }

        public void OnQuantityChanged(float value)
        {
            _quantity = Mathf.RoundToInt(value);
            QuantityText.text = _quantity.ToString();

            if (_selectedItem != null)
                PricePanel.Initialize(_selectedItem.Product.Type, _selectedItem.Product.Price * _quantity);
        }

        public void OnFilterSelected(int value)
        {
            UpdateItems();
        }

        private void UpdateStats()
        {
            MoneyText.text = _playerResources.Money.ToString();
            StarPanel.gameObject.SetActive(CurrencyExtensions.PremiumCurrencyAllowed);
            StarText.text = _playerResources.Stars.ToString();
        }

        private void UpdateButtons()
        {
            var quantity = _selectedItem != null ? _selectedItem.Product.Quantity : 0;
            var price = _selectedItem != null ? _selectedItem.Product.Price.Amount : 0;

            if (quantity > 1 && price > 0)
            {
                QuantityPanel.gameObject.SetActive(true);
                QuantitySlider.gameObject.SetActive(true);
                QuantitySlider.maxValue = quantity;
                QuantitySlider.value = 1;
                QuantitySlider.onValueChanged.Invoke(1);
                ScrapButton.interactable = true;
            }
            else
            {
                QuantityPanel.gameObject.SetActive(false);
                QuantitySlider.gameObject.SetActive(false);
                ScrapButton.interactable = price > 0;
            }

            UpdateItemDescription(_selectedItem != null ? _selectedItem.Product : null);
        }

        private void UpdateItemDescription(IProduct product)
        {
            if (product != null)
            {
                DescriptionPanel.gameObject.SetActive(true);
                Icon.sprite = product.Type.GetIcon(_resourceLocator);
                Icon.color = product.Type.Color;
                NameText.text = _localization.GetString(product.Type.Name);
                DescrtiptionText.gameObject.SetActive(!string.IsNullOrEmpty(DescrtiptionText.text = product.Type.Description));
                NameText.color = DescrtiptionText.color = ColorTable.QualityColor(product.Type.Quality);
                PricePanel.Initialize(product.Type, product.Price);
                PricePanel.gameObject.SetActive(product.Price.Amount > 0);
            }
            else
            {
                DescriptionPanel.gameObject.SetActive(false);
            }
        }

        private int UpdatePlayerItems(bool clearSelection = false)
        {
            ContentFiller.InitializeItems(_inventory.Items.Where(IsItemVisible).OrderBy(item => item.Type.Id), true, clearSelection);
            ItemList.RefreshContent();
            return ContentFiller.GetItemCount();
        }

        private bool IsItemVisible(IProduct item)
        {
            var filter = (Filter)FilterGroup.Value;
            if (filter == Filter.All)
                return true;

            if (item.Type is ComponentItem)
            {
                var component = ((ComponentItem)item.Type).Component;
                if (component.Data.DisplayCategory == ComponentCategory.Weapon)
                    return filter == Filter.Weapon;
                else
                    return filter == Filter.Module;
            }

            if (item.Type is ShipItem)
                return filter == Filter.Ship;

            if (item.Type is SatelliteItem)
                return filter == Filter.Module;

            if (item.Type is ArtifactItem || item.Type is FuelItem)
                return filter == Filter.Resource;

            return filter == Filter.Other;
        }

        private void UpdateItems()
        {
            var count = UpdatePlayerItems();
            EmptyLabel.gameObject.SetActive(count == 0);

            _selectedItem = null;
            if (ItemsGroup.AnyTogglesOn())
            {
                var first = ItemsGroup.ActiveToggles().FirstOrDefault();
                foreach (var item in ItemsGroup.ActiveToggles().Skip(1))
                    item.isOn = false;
                if (first != null)
                    first.onValueChanged.Invoke(true);
            }

            UpdateButtons();
            UpdateStats();
        }

        private int _quantity;
        private IInventory _inventory;
        private ViewModel.Common.InventoryItem _selectedItem;
    }
}
