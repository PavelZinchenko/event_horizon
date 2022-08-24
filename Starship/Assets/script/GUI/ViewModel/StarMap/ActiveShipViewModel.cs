//using System.Linq;
//using UnityEngine;
//using UnityEngine.UI;
//using Constructor;
//using GameServices.Player;
//using Services.Localization;
//using Services.Messenger;
//using Zenject;

//namespace ViewModel
//{
//	public class ActiveShipViewModel : MonoBehaviour
//	{
//        [Inject] private readonly PlayerInventory _playerInventory;
//        [Inject] private readonly PlayerFleet _playerFleet;
//	    [Inject] private readonly IMessenger _messenger;
//	    [Inject] private readonly ILocalization _localization;

//		public float Speed = 0.01f;
//		public Image Icon;
//		public Text NameLabel;
//		public Text LevelText;
//		public Text ExpText;
//		public Text BonusText;
//		public Slider ExpSlider;
//		public Button UpgradeButton;
//		public Image LeftComponentIcon;
//		public Image RightComponentIcon;
//		public Toggle LeftComponentToggle;
//		public Toggle RightComponentToggle;
//		public ToggleGroup CompanionToggleGroup;

//		public void OnShipSelected()
//		{
//            // TODO
//			//var activeShip = _playerFleet.ActiveShip;
//			//if (activeShip == null)
//			//	return;
			
//			//Icon.sprite = activeShip.Sprite;

//			//UpdateCompanions(activeShip);
//			//UpdateDescription(activeShip);
//		}

//		public void OnCompanionPanelClosed()
//		{
//			CompanionToggleGroup.SetAllTogglesOff();
//		}

//		public void OnCompanionSelected(string id)
//		{
//            // TODO
//			//var activeShip = _playerFleet.ActiveShip;
//			//var activeToggle = CompanionToggleGroup.ActiveToggles().FirstOrDefault();

//			//if (activeShip == null || activeToggle == null)
//			//	return;

//			//Constructor.CompanionLocation location;
//			//if (activeToggle == LeftComponentToggle)
//			//	location = Constructor.CompanionLocation.Left;
//			//else
//			//	location = Constructor.CompanionLocation.Right;

//			//CompanionSpecification companion = null;
//			//if (!string.IsNullOrEmpty(id))
//			//{
//			//	var companionData = ComponentCache.GetCompanion(id);
//			//	if (companionData == null || !activeShip.IsSuitableCompanionSize(companionData.Data.Size))
//			//		throw new System.ArgumentException("cannot install " + id + " in " + activeShip.Data.name);

//			//	_playerInventory.Companions.Remove(id);
//			//	companion = new CompanionSpecification(companionData, Enumerable.Empty<IntegratedComponent>(), location);
//			//}

//			//var oldValue = companion != null ? activeShip.ReplaceCompanion(companion) : activeShip.RemoveCompanion(location);
//			//if (oldValue != null)
//			//{
//			//	_playerInventory.Companions.Add(oldValue.Companion.name);
//			//	foreach (var item in oldValue.Components)
//			//		_playerInventory.Components.Add(item.Info);
//			//}

//			//_playerFleet.SaveShips();
//			//_playerInventory.SaveComponents();

//			//CompanionToggleGroup.SetAllTogglesOff();
//			//UpdateCompanions(activeShip);
//		}

//		private void UpdateCompanions(GameModel.Ship activeShip)
//		{
//			LeftComponentToggle.isOn = false;
//			RightComponentToggle.isOn = false;
//			var leftCompanion = activeShip.GetCompanion(Constructor.CompanionLocation.Left);
//			var rightCompanion = activeShip.GetCompanion(Constructor.CompanionLocation.Right);
//			LeftComponentIcon.gameObject.SetActive(leftCompanion != null);
//			RightComponentIcon.gameObject.SetActive(rightCompanion != null);
//			if (leftCompanion != null)
//				LeftComponentIcon.sprite = leftCompanion.Companion.Data.Sprite;
//			if (rightCompanion != null)
//				RightComponentIcon.sprite = rightCompanion.Companion.Data.Sprite;
//		}

//		private void UpdateDescription(GameModel.Ship ship)
//		{
//			var rank = ship != null ? ship.Level : 0;

//			BonusText.gameObject.SetActive(rank > 0);
//			BonusText.text = _localization.GetString("$LevelBonus",
//				(Mathf.RoundToInt((ship.Experience.PowerMultiplier - 1f)*100)).ToString());

//			var expCurrent = ship.Experience.ExpFromLastLevel;
//			var expTotal = ship.Level < Maths.Experience.MaxPlayerRank ? ship.Experience.NextLevelCost : 0;

//			ExpText.text = expTotal > 0 ? expCurrent + " / " + expTotal : string.Empty;
//			ExpSlider.value = expTotal > 0 ? (float)expCurrent / (float)expTotal : 0;

//			UpgradeButton.gameObject.SetActive(ship != null);
//			NameLabel.text = ship == null ? string.Empty : ship.Name;
//			if (ship == null || rank == 0)
//			{
//				LevelText.gameObject.SetActive(false);
//			}
//			else
//			{
//				LevelText.gameObject.SetActive(true);
//				LevelText.text = _localization.GetString("$ShipLevel", rank.ToString());
//			}
//		}

//		private float _lastUpadteTime;
//		private Image _image;
//	}
//}
