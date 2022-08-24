using System;
using DataModel.Technology;
using GameServices.Research;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Services.Localization;
using Services.Reources;
using Zenject;

namespace ViewModel
{
	public class TechItemViewModel : MonoBehaviour
	{
	    [Inject] private readonly Research _research;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private Toggle Toggle;
        [SerializeField] private Image Icon;
        [SerializeField] private Text Name;
        [SerializeField] private Text Description;
        [SerializeField] private Text PriceText;
        [SerializeField] private Graphic[] UiElements;
	    [SerializeField] private Color ResearchedColor;
        [SerializeField] private Color AvailableColor;
        [SerializeField] private Color NotAvailableColor;
        [SerializeField] private Color HiddenColor;
	    [SerializeField] private Sprite HiddenIcon;

        public TechEvent OnTechSelectedEvent = new TechEvent();
		public TechEvent OnTechDeselectedEvent = new TechEvent();
		
		[Serializable]
		public class TechEvent : UnityEvent<ITechnology>
		{
		}

		public void Initialize(ITechnology technology)
		{
			_technology = technology;
			var researched = _research.IsTechResearched(technology);
			var available = _research.IsTechAvailable(technology) && !technology.Hidden;
		    var hidden = technology.Hidden && !researched;

		    var color = researched ? ResearchedColor : hidden ? HiddenColor : available ? AvailableColor : NotAvailableColor;
		    foreach (var element in UiElements)
		        element.color = color;

			Icon.sprite = hidden ? HiddenIcon : technology.GetImage(_resourceLocator);
			
			if (!available && !researched)
			{
				Name.text = "?????";
				Description.text = string.Empty;
				Icon.color = Color.black;
				Toggle.interactable = false;
			}
			else
			{
				Name.text = _localization.GetString(technology.GetName(_localization));
				Description.text = _localization.GetString(technology.GetDescription(_localization));
				Icon.color = technology.Color;
				Toggle.interactable = true;
			}
			
			PriceText.text = researched || hidden ? string.Empty : technology.Price.ToString();
		}

		public void OnToggleValueChanged(bool value)
		{
            if (_technology == null) return;

			if (value)
				OnTechSelectedEvent.Invoke(_technology);
			else
				OnTechDeselectedEvent.Invoke(_technology);
		}

		private ITechnology _technology;
	}
}
