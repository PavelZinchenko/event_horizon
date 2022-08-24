using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Extensions;
using UnityEngine;
using UnityEngine.UI;
using Services.Messenger;
using Services.ObjectPool;
using Zenject;

namespace ViewModel
{
	public class ResearchPanelViewModel : MonoBehaviour
	{
	    [Inject] private readonly IMessenger _messenger;
	    [Inject] private readonly GameObjectFactory _factory;
	    [Inject] private readonly IDatabase _database;

		public TechTreePanelViewModel TechTree;
		public LayoutGroup FactionsLayout;
		public ToggleGroup FactionsGroup;

		public void OnFactionSelected(Faction faction)
		{
			TechTree.Initialize(faction);
		}

		private void Start()
		{
            _messenger.AddListener(EventType.TechResearched, UpdateFactions);
		}

		private void OnEnable()
		{
			UpdateFactions();
		    FactionsLayout.transform.Cast<Transform>().Select(child => child.GetComponent<FactionViewModel>()).First(item => item != null).Toggle.isOn = true;
		}

		private void UpdateFactions()
		{
			if (gameObject.activeSelf)
				FactionsLayout.InitializeElements<FactionViewModel, Faction>(_database.FactionList.Visible(), UpdateFaction, _factory);
		}

		private void UpdateFaction(FactionViewModel item, Faction faction)
		{
			item.SetFaction(faction);
		    item.Toggle.isOn = false;
		}
	}
}
