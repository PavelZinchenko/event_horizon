using System.Linq;
using Domain.Quests;
using Economy.ItemType;
using Economy.Products;
using Galaxy;
using GameDatabase.DataModel;
using UnityEngine;
using UnityEngine.UI;
using GameModel.Quests;
using GameServices.Player;
using GameServices.Quests;
using GameStateMachine.States;
using Services.Localization;
using Services.Messenger;
using Session;
using Utils;
using Zenject;

namespace ViewModel
{
	public class FactionPanelViewModel : MonoBehaviour
	{
        [Inject] private readonly ItemTypeFactory _factory;
        [Inject] private readonly IMessenger _messenger;
	    [Inject] private readonly MotherShip _motherShip;
	    [Inject] private readonly IQuestManager _questManager;
	    [Inject] private readonly OpenShopSignal.Trigger _openShopTrigger;
	    [Inject] private readonly InventoryFactory _inventoryFactory;
	    [Inject] private readonly ISessionData _session;
	    [Inject] private readonly ILocalization _localization;
        [Inject] private readonly QuestEventSignal.Trigger _questEventTrigger;
	    [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;

		[SerializeField] private GameObject CaptureButton;
	    [SerializeField] private GameObject CaptureDescription;
	    [SerializeField] private GameObject MilitaryPowerPanel;
	    [SerializeField] private GameObject ReputationPanel;
        [SerializeField] private GameObject ShopButton;
	    [SerializeField] private GameObject CraftButton;
	    [SerializeField] private GameObject ShipyardButton;
	    [SerializeField] private GameObject MissionButton;
	    [SerializeField] private Text FactionName;
	    [SerializeField] private Text PowerText;
	    [SerializeField] private Text ReputationText;

        public void OpenStore()
		{
            _openShopTrigger.Fire(_inventoryFactory.CreateFactionInventory(_motherShip.CurrentStar.Region), _inventoryFactory.CreatePlayerInventory());
        }

		public void CaptureBase()
		{
			OptimizedDebug.Log("FactionPanelViewModel.CaptureBase");

			_motherShip.CurrentStar.CaptureBase();
		}

        public bool MissionsAvailable
        {
            get
            {
                if (_motherShip.CurrentStar.Region.Faction.Hostile) return false;
                return !_questManager.Quests.Any(item => item.IsFactionMission(_motherShip.CurrentStar.Id));
            }
        }

        public void TakeMission()
	    {
            MissionButton.gameObject.SetActive(false);
	        _questEventTrigger.Fire(new StarEventData(QuestEventType.FactionMissionAccepted, _motherShip.CurrentStar.Region.HomeStar));
        }

		private void OnEnable()
		{
			var region = _motherShip.CurrentStar.Region;

		    FactionName.text = _localization.GetString(region.Faction.Name);
            FactionName.color = region.Faction.Color;

		    var reputation = _session.Quests.GetFactionRelations(region.HomeStar);

            if (region.IsCaptured)
		    {
		        CaptureButton.gameObject.SetActive(false);
		        CaptureDescription.gameObject.SetActive(false);
		        MilitaryPowerPanel.gameObject.SetActive(false);
		        ReputationPanel.gameObject.SetActive(false);
		        ShopButton.SetActive(true);
		        CraftButton.SetActive(true);
		        ShipyardButton.SetActive(true);
		        MissionButton.gameObject.SetActive(false);
                return;
		    }

            CaptureButton.gameObject.SetActive(true);
		    CaptureDescription.gameObject.SetActive(true);
		    MilitaryPowerPanel.gameObject.SetActive(true);
		    ReputationPanel.gameObject.SetActive(!region.Faction.Hostile);
		    ReputationText.text = reputation > 0 ? "+" + reputation : reputation.ToString();
		    PowerText.text = Mathf.RoundToInt(region.BaseDefensePower * 100) + "%";

            MissionButton.gameObject.SetActive(MissionsAvailable);

			ShopButton.SetActive(reputation >= 5);
			CraftButton.SetActive(reputation >= 60);
            ShipyardButton.SetActive(reputation >= 90);
		}
	}
}
