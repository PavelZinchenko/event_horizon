using System;
using DataModel.Technology;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Zenject;

namespace ViewModel
{
	public class CraftListItem : MonoBehaviour
	{
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public Toggle Toggle;
		public Image Icon;
		public Text Name;
		public Text Description;
	    public GameObject RequiredLevelPanel;
	    public Text RequiredLevelText;

		public TechEvent OnTechSelectedEvent = new TechEvent();
		public TechEvent OnTechDeselectedEvent = new TechEvent();
		
		[Serializable]
		public class TechEvent : UnityEvent<ITechnology>
		{
		}
		
		public void InitializeForCraft(ITechnology technology, int workshopLevel)
		{
			_technology = technology;
			Icon.sprite = technology.GetImage(_resourceLocator);
			Name.text = technology.GetName(_localization);
			Description.text = technology.GetDescription(_localization);
			Icon.color = technology.Color;

		    var techLevel = GameModel.Craft.GetWorkshopLevel(technology);
            var available = techLevel <= workshopLevel;
            RequiredLevelPanel.gameObject.SetActive(!available);
            RequiredLevelText.text = techLevel.ToString();
		}
		
		public void OnToggleValueChanged(bool value)
		{
			if (value)
				OnTechSelectedEvent.Invoke(_technology);
			else
				OnTechDeselectedEvent.Invoke(_technology);
		}
		
		private ITechnology _technology;
	}
}
