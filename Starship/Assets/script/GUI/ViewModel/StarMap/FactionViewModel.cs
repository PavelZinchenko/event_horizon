using System;
using Domain.Player;
using GameDatabase.DataModel;
using GameServices.Research;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Services.Localization;
using Zenject;

namespace ViewModel
{
	public class FactionViewModel : MonoBehaviour
	{
	    [Inject] private readonly Research _research;
	    [Inject] private readonly ILocalization _localization;
	    [Inject] private readonly StarMapManager _starMapManager;

		public Image Icon;
		public Image Background;
		public Toggle Toggle;
		public Text Name;
		public Text ResearchPointText;
		public GameObject ResearchPointPanel;

		public FactionEvent OnFactionSelectedEvent = new FactionEvent();
		public FactionEvent OnFactionDeselectedEvent = new FactionEvent();

		[Serializable]
		public class FactionEvent : UnityEvent<Faction>
        {
        }

        public void SetFaction(Faction faction)
		{
			_faction = faction;
		    var unlocked = _starMapManager.IsFactionUnlocked(faction);
			var color = faction.Color;
			Icon.color = color;
			Background.color = new Color(color.R, color.G, color.B, 0.5f);
			Name.text = unlocked ? _localization.GetString(faction.Name) : "???";
			var researchPoints = _research.GetAvailablePoints(faction);
			ResearchPointPanel.gameObject.SetActive(researchPoints != 0);
			ResearchPointText.text = (researchPoints > 0 ? "+" : "") + researchPoints;
			ResearchPointText.color = researchPoints > 0 ? Color.white : Color.red;
		    Toggle.interactable = unlocked;
		}

		private void OnValueChanged(bool value)
		{
			if (value)
				OnFactionSelectedEvent.Invoke(_faction);
			else
				OnFactionDeselectedEvent.Invoke(_faction);
		}

		private void OnEnable()
		{
			Toggle.onValueChanged.AddListener(OnValueChanged);
		}

		private Faction _faction = Faction.Undefined;
	}
}
