using System.Linq;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase.Enums;
using UnityEngine;
using UnityEngine.UI;
using GameModel;
using GameModel.Quests;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using Services.Gui;
using Services.Localization;
using Services.Messenger;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	namespace Dialogs
	{
		public class MarketDialog : MonoBehaviour
		{
		    [Inject] private readonly IMessenger _messenger;
		    [Inject] private readonly ILocalization _localization;
            [Inject] private readonly ISoundPlayer _soundPlayer;
		    [Inject] private readonly PlayerResources _playerResources;
		    [Inject] private readonly IResourceLocator _resourceLocator;
            [Inject] private readonly GuiHelper _helper;

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
		    [SerializeField] private Toggle BuyToggle;
		    [SerializeField] private Toggle SellToggle;
		    [SerializeField] private AudioClip Sound;
		    [SerializeField] private Button BuyButton;
		    [SerializeField] private Button SellButton;
		    [SerializeField] private Button _sellJunkButton;
		    [SerializeField] private GameObject QuantityPanel;
		    [SerializeField] private Slider QuantitySlider;
			[SerializeField] private RadioGroupViewModel FilterGroup;
			[SerializeField] private Common.PricePanel PricePanel;
			[SerializeField] private GameObject DescriptionPanel;
			[SerializeField] private Text QuantityText;
            [SerializeField] private Text NameText;
            [SerializeField] private Text DescrtiptionText;
            [SerializeField] private Image Icon;
			[SerializeField] private GameObject EmptyLabel;
			[SerializeField] private Text MoneyText;
		    [SerializeField] private Text StarText;
		    [SerializeField] private GameObject StarPanel;

            [SerializeField] MarketContentFiller ContentFiller;
            [SerializeField] ListScrollRect ItemList;

            public void Initialize(WindowArgs args)
            {
                _marketInventory = args.Get<IInventory>(0);
                _playerInventory = args.Get<IInventory>(1);

                _messenger.AddListener(EventType.IapItemsRefreshed, OnIapItemsChanged);
                _messenger.AddListener<int>(EventType.MoneyValueChanged, value => UpdateStats());
                _messenger.AddListener<int>(EventType.StarsValueChanged, value => UpdateStats());

                DescriptionPanel.gameObject.SetActive(false);
                BuyToggle.isOn = false;
                SellToggle.isOn = false;
                BuyToggle.isOn = true;
            }

            public void OnItemSelected(Common.InventoryItem item)
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
			
			public void BuyButtonClicked()
			{
				_selectedItem.Product.Buy(_quantity);
				_soundPlayer.Play(Sound);
				UpdateItems();
			}

			public void CloseButtonClicked()
			{
                GetComponent<IWindow>().Close();
			}
			
			public void SellButtonClicked()
			{
				_selectedItem.Product.Sell(_quantity);
				_soundPlayer.Play(Sound);
				UpdateItems();
			}

		    public void SellJunkButtonClicked()
		    {
		        _helper.ShowConfirmation(_localization.GetString("$SellAllTrashConfirmation"), () =>
		        {
		            var items = _playerInventory.Items.Where(IsJunk).ToArray();

		            foreach (var item in items)
		            {
		                var quantity = item.Quantity;
		                item.Sell(quantity);
		            }

		            _soundPlayer.Play(Sound);
		            UpdateItems();
		        });
		    }

		    private static bool IsJunk(IProduct product)
		    {
		        if (product.Quantity <= 0)
		            return false;
		        if (product.Type is ArtifactItem)
		            return false;

		        return product.Type.Quality < ItemQuality.Common;
		    }

            public void OnBuyToggleValueChanged(bool selected)
			{
			    if (!selected || !IsInitialized) return;

                _mode = Mode.Buy;
				FilterGroup.Value = (int)Filter.All;
				EmptyLabel.gameObject.SetActive(UpdateMarketItems(true) == 0);
				_selectedItem = null;
				ItemsGroup.SetAllTogglesOff();
				UpdateButtons();
				UpdateStats();
			}
			
			public void OnSellToggleValueChanged(bool selected)
			{
			    if (!selected || !IsInitialized) return;

                _mode = Mode.Sell;
				//if (FilterGroup.Value < 0)
				//	FilterGroup.Value = (int)Filter.Resource;

				EmptyLabel.gameObject.SetActive(UpdatePlayerItems(true) == 0);
				_selectedItem = null;
				ItemsGroup.SetAllTogglesOff();
				UpdateButtons();
			}
			
			public void OnQuantityChanged(float value)
			{
				_quantity = Mathf.RoundToInt(value);
				QuantityText.text = _quantity.ToString();

				if (_selectedItem != null)
					PricePanel.Initialize(_selectedItem.Product.Type, _selectedItem.Product.Price*_quantity);
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

		    private bool IsInitialized => _marketInventory != null && _playerInventory != null;

            private void UpdateButtons()
			{
				int quantity = 0;
				if (_mode == Mode.Buy)
				{
					BuyButton.gameObject.SetActive(true);
					SellButton.gameObject.SetActive(false);
                    _sellJunkButton.gameObject.SetActive(false);
					
					BuyButton.interactable =
						_selectedItem != null &&
						_selectedItem.Product.Price.IsEnough(_playerResources) &&
						_selectedItem.Product.Type.MaxItemsToConsume > 0;
					
					if (_selectedItem != null)
					{
						var product = _selectedItem.Product;
						if (product.Price.Amount > 0)
							quantity = Mathf.Min(product.Quantity, product.Type.MaxItemsToConsume, product.Price.GetMaxItemsToWithdraw(_playerResources));
						else
							quantity = Mathf.Min(product.Quantity, product.Type.MaxItemsToConsume);							
					}
				}
				else
				{
					BuyButton.gameObject.SetActive(false);
					SellButton.gameObject.SetActive(true);
					SellButton.interactable = _selectedItem != null;
				    _sellJunkButton.gameObject.SetActive(true);
                    _sellJunkButton.interactable = _playerInventory.Items.Any(IsJunk);
					quantity = _selectedItem != null ? _selectedItem.Product.Quantity : 0;
				}

				if (quantity > 1)
				{
					QuantityPanel.gameObject.SetActive(true);
					QuantitySlider.gameObject.SetActive(true);
					QuantitySlider.maxValue = quantity;
					QuantitySlider.value = 1;
					QuantitySlider.onValueChanged.Invoke(1);
				}
				else
				{
					QuantityPanel.gameObject.SetActive(false);
					QuantitySlider.gameObject.SetActive(false);
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
				}
				else
				{
					DescriptionPanel.gameObject.SetActive(false);
				}
			}
			
			private int UpdateMarketItems(bool clearSelection = false)
			{
                var filter = (Filter)FilterGroup.Value;
                ContentFiller.InitializeItems(_marketInventory.Items.Where(IsItemVisible).OrderBy(item => item.Type.Id), false, clearSelection);
                ItemList.RefreshContent();
				return ContentFiller.GetItemCount();
			}
			
			private int UpdatePlayerItems(bool clearSelection = false)
			{
                ContentFiller.InitializeItems(_playerInventory.Items.Where(IsItemVisible), true, clearSelection);
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
				var count = _mode == Mode.Buy ? UpdateMarketItems() : UpdatePlayerItems();
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

			private void OnIapItemsChanged()
			{
                UnityEngine.Debug.Log("MarketPanel: OnIapItemsChanged");
				_marketInventory.Refresh();
				UpdateItems();
			}
			
			private void UpdateMarketItem(Common.InventoryItem item, IProduct product)
			{
				item.Initialize(product, !product.Price.IsEnough(_playerResources), _resourceLocator);
			}
			
			private void UpdatePlayerItem(Common.InventoryItem item, IProduct product)
			{
				item.Initialize(product, false, _resourceLocator);
			}

			private void Update()
			{
				if (Input.GetKeyDown(KeyCode.Escape))
					CloseButtonClicked();
			}

			private int _quantity;
			private Mode _mode;
			private IInventory _playerInventory;
			private IInventory _marketInventory;
			private Common.InventoryItem _selectedItem;

			private enum Mode { Buy, Sell }
		}
	}
}
