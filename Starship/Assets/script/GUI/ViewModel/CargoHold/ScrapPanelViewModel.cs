using System.Linq;
using System.Collections.Generic;
using Economy.ItemType;
using Economy.Products;
using GameDatabase.Enums;
using GameServices.Gui;
using GameServices.Player;
using Services.Audio;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;
using AudioClip = UnityEngine.AudioClip;
using ComponentInfo = Constructor.ComponentInfo;

namespace ViewModel
{
	public class ScrapPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly ISoundPlayer _soundPlayer;
	    [Inject] private readonly PlayerResources _playerResources;
	    [Inject] private readonly PlayerFleet _playerFleet;
	    [Inject] private readonly PlayerInventory _playerInventory;
	    [Inject] private readonly ItemTypeFactory _factory;
	    [Inject] private readonly GuiHelper _helper;

        [SerializeField] ToggleGroup _itemGroup;
        [SerializeField] AudioClip _scrapSound;
        [SerializeField] Button _scrapButton;
        [SerializeField] Button _infoButton;
        [SerializeField] ListScrollRect _itemList;
        [SerializeField] CargoHoldContentFiller _contentFiller;

        public void InfoButtonPressed()
        {
            var item = SelectedItem;
            if (item == null)
                return;

            _helper.ShowItemInfoWindow(item.Product);
		}

		public void ScrapButtonPressed()
		{
            var item = SelectedItem;
            if (item == null)
                return;

            item.Product.Withdraw(1);
            _playerResources.Money += Mathf.RoundToInt(item.Product.Price.Amount * UnityEngine.Random.Range(priceScaleMin, priceScaleMax));
			_soundPlayer.Play(_scrapSound);
			UpdateItems();
		}

        public void OnItemSelected(bool selected)
        {
            if (!selected)
                return;

            var item = SelectedItem;
            if (item == null)
                return;

            _contentFiller.OnItemSelected(item);
            _scrapButton.interactable = selected;
            _infoButton.interactable = selected;
        }

        private CargoHoldItem SelectedItem
        {
            get
            {
                var toggle = _itemGroup.ActiveToggles().FirstOrDefault();
                return toggle ? toggle.GetComponent<CargoHoldItem>() : null;
            }
        }

		private void OnEnable()
		{
            OptimizedDebug.Log("ScrapPanelViewModel.OnEnable");
            UpdateItems();
		}

		private void UpdateItems()
		{
            _contentFiller.Initialize(Items);
            _itemList.RefreshContent();

//			_activeItems.Clear();
//			foreach (var ship in Game.Session.Player.Fleet.GetAllHangarShips())
//				_activeItems.Add(ship);
//
//			bool selected = false;
//			var enumerator = Items.GetEnumerator();
//			RectTransform item = null;
//			foreach (Transform transform in ItemsGroup.transform)
//			{
//				item = transform.GetComponent<RectTransform>();
//				if (enumerator.MoveNext())
//				{
//					var current = enumerator.Current;
//					UpdateItem(item, current);
//					selected |= current == SelectedItem;
//				}
//				else
//					item.gameObject.SetActive(false);
//			}
//			
//			while (enumerator.MoveNext())
//			{
//				var current = enumerator.Current;
//				var newItem = (RectTransform)Instantiate(item);
//				newItem.SetParent(item.parent);
//				newItem.localScale = Vector3.one;
//				UpdateItem(newItem, current);
//				selected |= current == SelectedItem;
//			}
//
//			if (!selected)
//			{
//				ItemsGroup.SetAllTogglesOff();
//				SelectedItem = null;
//			}
		}

        private IEnumerable<IProduct> Items
		{
			get
			{
				foreach (var ship in _playerFleet.Ships.Where(item => item.Model.Category != ShipCategory.Special))
                    yield return new Product(_factory.CreateShipItem(ship));
                foreach (var item in _playerInventory.Satellites.Items)
                    yield return new Product(_factory.CreateSatelliteItem(item.Key), item.Value);
			    foreach (var item in _playerInventory.Components.Items.OrderBy(component => component.Key.SerializeToInt64()))
                    yield return new Product(_factory.CreateComponentItem(item.Key), item.Value);
			}
		}
		
//		private void UpdateItem(RectTransform item, object data)
//		{
//			item.gameObject.SetActive(true);
//			var viewModel = item.GetComponent<WorkshopPanelItemViewModel>();
//			viewModel.ToggleButton.onValueChanged.RemoveAllListeners();
//			viewModel.ToggleButton.isOn = SelectedItem == data;
//			viewModel.ToggleButton.group = ItemsGroup;
//			viewModel.ToggleButton.onValueChanged.AddListener((value) => SelectedItemChanged(data, value));
//
//			if (data is GameModel.Ship)
//				UpdateItem(viewModel, (GameModel.Ship)data);
//			if (data is Constructor.CompanionWrapper)
//				UpdateItem(viewModel, (Constructor.CompanionWrapper)data);
//			else if (data is ComponentInfo)
//				UpdateItem(viewModel, (ComponentInfo)data);
//		}
//
//		private void UpdateItem(WorkshopPanelItemViewModel item, GameModel.Ship ship)
//		{
//			item.Icon.sprite = ship.Icon;
//			item.Icon.rectTransform.localScale = Vector3.one*ship.IconScale;
//			item.Icon.color = Color.white;
//			item.InfoText.gameObject.SetActive(false);
//			item.RankPanel.SetActive(ship.Level > 0);
//			item.RankText.text = ship.Level.ToString();
//			item.ItemInUsePanel.SetActive(_activeItems.Contains(ship));
//		}
//
//		private void UpdateItem(WorkshopPanelItemViewModel item, ComponentInfo component)
//		{
//			item.Icon.sprite = component.Data.Icon;
//			item.Icon.rectTransform.localScale = Vector3.one*ModuleIconScale;
//			item.Icon.color = component.Data.Color;
//			item.InfoText.gameObject.SetActive(true);
//			item.InfoText.text = Game.Session.Player.Fleet.Components.GetQuantity(component).ToString();
//			item.RankPanel.SetActive(false);
//			item.ItemInUsePanel.SetActive(_activeItems.Contains(component));
//		}
//
//		private void UpdateItem(WorkshopPanelItemViewModel item, Constructor.CompanionWrapper companion)
//		{
//			item.Icon.sprite = companion.Data.Sprite;
//			item.Icon.rectTransform.localScale = Vector3.one*ModuleIconScale;
//			item.Icon.color = Color.white;
//			item.InfoText.gameObject.SetActive(true);
//			item.InfoText.text = Game.Session.Player.Fleet.Companions.GetQuantity(companion.name).ToString();
//			item.RankPanel.SetActive(false);
//			item.ItemInUsePanel.SetActive(_activeItems.Contains(companion));
//		}
//		
//		private void SelectedItemChanged(object data, bool value)
//		{
//			SelectedItem = value ? data : null;
//		}
//		
//		private object SelectedItem 
//		{
//			get { return _selectedItem; }
//			set
//			{
//				_selectedItem = value;
//				UpdateFooter();
//			}
//		}
//
//		private void UpdateFooter()
//		{
//			if (_selectedItem != null)
//			{
//				ScrapButton.interactable = true;
//				InfoButton.interactable = true;
//				var price = ScrapPrice;				
//				NameText.gameObject.SetActive(true);
//				NameText.text = ItemName;
//                DescriptionText.gameObject.SetActive(!string.IsNullOrEmpty(DescriptionText.text = ItemDescription));
//		        NameText.color = DescriptionText.color = ColorTable.QualityColor(ItemQuality);
//			}
//			else
//			{
//				NameText.gameObject.SetActive(false);
//				DescriptionText.gameObject.SetActive(false);
//				ScrapButton.interactable = false;
//				InfoButton.interactable = false;
//			}
//		}
//
//		private int ScrapPrice
//		{
//			get
//			{
//				int price = 0;
//				if (_selectedItem is GameModel.Ship)
//					price = ((GameModel.Ship)_selectedItem).Price;
//				else if (_selectedItem is Constructor.CompanionWrapper)
//					price = ((Constructor.CompanionWrapper)_selectedItem).Price;
//				else if (_selectedItem is ComponentInfo)
//					price = ((ComponentInfo)_selectedItem).Price;
//				return price;
//			}
//		}
//
//		private string ItemName
//		{
//			get
//			{
//				if (_selectedItem is GameModel.Ship)
//					return ((GameModel.Ship)_selectedItem).Name;
//				if (_selectedItem is Constructor.CompanionWrapper)
//					return ServiceLocator.Localization.GetString(((Constructor.CompanionWrapper)_selectedItem).Data.Name);
//				if (_selectedItem is ComponentInfo)
//					return ServiceLocator.Localization.GetString(((ComponentInfo)_selectedItem).Data.Name);
//				return string.Empty;
//			}
//		}
//
//        private string ItemDescription
//        {
//            get
//            {
//                if (_selectedItem is ComponentInfo)
//                    return ((ComponentInfo)_selectedItem).CreateModification().Description;
//                return string.Empty;
//            }
//        }
//
//        private ItemQuality ItemQuality
//        {
//            get
//            {
//                if (_selectedItem is GameModel.Ship)
//                    return ((GameModel.Ship)_selectedItem).Data.Rarity.ToItemQuality();
//                if (_selectedItem is CompanionWrapper)
//                    return ItemQuality.Medium;
//                if (_selectedItem is ComponentInfo)
//                    return ((ComponentInfo)_selectedItem).ItemQuality;
//                return ItemQuality.Common;
//            }
//        }

        private static float priceScaleMin = 0.2f;
		private static float priceScaleMax = 0.4f;
	}
}
