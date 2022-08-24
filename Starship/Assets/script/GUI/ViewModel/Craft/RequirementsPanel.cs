using UnityEngine;
using UnityEngine.UI;
using DataModel.Technology;
using GameServices.Player;
using Zenject;

namespace ViewModel
{
	//public class RequirementsPanel : MonoBehaviour
	//{
	//    [Inject] private readonly PlayerResources _playerResources;

	//	public Text LevelText;
	//	public Toggle LevelToggle;
	//	public Text PriceText;
	//	public Toggle PriceToggle;
	//	public Toggle ResourcesToggle;
	//	public RequiredResources RequiredResources;

	//	public void Initialize(ITechnology tech, int workshopLevel)
	//	{
	//		if (tech == null)
	//		{
	//			LevelText.text = "-";
	//			PriceText.text = "-";
	//			RequiredResources.UpdateItems(null);
	//			return;
	//		}
			
	//		var requiredLevel = GameModel.Craft.GetWorkshopLevel(tech);
	//		LevelText.text = requiredLevel.ToString();
	//		var haveLevel = workshopLevel >= requiredLevel;
	//		LevelToggle.isOn = haveLevel;
	//		RequiredResources.UpdateItems(tech);
	//		var haveResources = GameModel.CraftingResources.IsEnoughToCraft(tech);
	//		ResourcesToggle.isOn = haveResources;
	//		var price = tech.CraftPrice;
	//		PriceText.text = price.ToString();
	//		var haveMoney = price.IsEnough(_playerResources);
	//		PriceToggle.isOn = haveMoney;
			
	//		//CraftButton.interactable = haveLevel && haveLevel && haveResources;
	//	}
	//}
}
