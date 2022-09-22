using Combat.Domain;
using GameServices.LevelManager;
using Scripts.GameStateMachine;
using Utils;
using Session;
using Zenject;
using GameServices.Quests;
using Galaxy;
using GameServices.Player;
using Services.Gui;
using Constructor.Ships;
using Domain.Quests;
using Game.Exploration;
using GameDatabase.DataModel;
using GameModel.Quests;
using Session.Content;
using UniRx;

namespace GameStateMachine.States
{
    public class StarMapState : BaseState, IQuestActionProcessor
    {
        [Inject]
        public StarMapState(
			IStateMachine stateMachine,
            GameStateFactory gameStateFactory,
            ILevelLoader levelLoader,
			IQuestManager questManager,
			ISessionData session,
            IGuiManager guiManager,
            MotherShip motherShip,
            PlayerResources playerResources,
			StarData starData,
            InventoryFactory inventoryFactory,
            SupplyShip supplyShip,
            RetreatSignal retreatSignal,
			StartTravelSignal startTravelSignal, 
			StartBattleSignal startBattleSignal,
			QuestActionRequiredSignal questActionRequiredSignal,
			QuestEventSignal.Trigger questEventTrigger,
            OpenSkillTreeSignal openSkillTreeSignal,
            OpenConstructorSignal openConstructorSignal,
            OpenShopSignal openShopSignal,
            OpenWorkshopSignal openWorkshopSignal,
			OpenShipyardSignal openShipyardSignal,
            OpenEhopediaSignal openEhopediaSignal,
            StartExplorationSignal startExplorationSignal,
            PlayerPositionChangedSignal playerPositionChangedSignal,
            SupplyShipActivatedSignal supplyShipActivatedSignal,
            ExitSignal exitSignal,
            EscapeKeyPressedSignal escapeKeyPressedSignal)
            : base(stateMachine, gameStateFactory, levelLoader)
        {
			_questManager = questManager;
            _questEventTrigger = questEventTrigger;
			_session = session;
			_starData = starData;
            _motherShip = motherShip;
            _playerResources = playerResources;
            _inventoryFactory = inventoryFactory;
            _guiManager = guiManager;
            _supplyShip = supplyShip;
			_retreatSignal = retreatSignal;
			_retreatSignal.Event += OnRetreat;
            _startTravelSignal = startTravelSignal;
            _startTravelSignal.Event += OnStartTravel;
			_startBattleSignal = startBattleSignal;
            _startBattleSignal.Event += OnStartBattle;
			_questActionRequiredSignal = questActionRequiredSignal;
			_questActionRequiredSignal.Event += OnQuestActionRequired;
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
            _openSkillTreeSignal = openSkillTreeSignal;
            _openSkillTreeSignal.Event += OnOpenSkillTree;
            _openConstructorSignal = openConstructorSignal;
            _openConstructorSignal.Event += OnOpenConstructor;
            _openShopSignal = openShopSignal;
            _openShopSignal.Event += OnOpenShop;
            _openWorkshopSignal = openWorkshopSignal;
            _openWorkshopSignal.Event += OnOpenWorkshop;
            _escapeKeyPressedSignal = escapeKeyPressedSignal;
            _escapeKeyPressedSignal.Event += OnEscapePressed;
            _playerPositionChangedSignal = playerPositionChangedSignal;
            _playerPositionChangedSignal.Event += OnPlayerPositionChanged;
            _openShipyardSignal = openShipyardSignal;
            _openShipyardSignal.Event += OnOpenShipyard;
            _startExplorationSignal = startExplorationSignal;
            _startExplorationSignal.Event += OnStartExploration;
            _openEhopediaSignal = openEhopediaSignal;
            _openEhopediaSignal.Event += OnOpenEhopedia;
            _supplyShipActivatedSignal = supplyShipActivatedSignal;
            _supplyShipActivatedSignal.Event += OnSupplyShipActivated;
        }

        public override StateType Type => StateType.StarMap;

        protected override LevelName RequiredLevel => LevelName.StarMap;

        protected override void OnActivate()
		{
            if (!string.IsNullOrEmpty(DesiredWindowOnResume))
            {
                var id = DesiredWindowOnResume;
                DesiredWindowOnResume = string.Empty;
                _guiManager.OpenWindow(id);
            }

            CheckStatus();
        }

        private void OnPlayerPositionChanged(int position)
        {
            if (IsActive) CheckStatus();
        }

        private void CheckStatus()
        {
            UpdateQuests();
            if (CheckStarGuardian()) return;
            ShowOutOfFuelDialog();
        }

        private void OnRetreat()
		{
			if (!IsActive)
				throw new BadGameStateException();

			StateMachine.LoadAdditionalState(StateFactory.CreateRetreatState());
		}

        private void OnStartTravel(int destination)
        {
            if (!IsActive)
                throw new BadGameStateException();

            var requiredFuel = _motherShip.CalculateRequiredFuel(_motherShip.Position, destination);
            if (_motherShip.ViewMode != ViewMode.GalaxyMap && requiredFuel < 3 && _playerResources.Fuel >= requiredFuel)
            {
                StateMachine.LoadAdditionalState(StateFactory.CreateTravelState(destination));
                return;
            }

            StateMachine.LoadAdditionalState(StateFactory.CreateDialogState(Gui.StarMap.WindowNames.FlightConfirmationDialog, new WindowArgs(destination),
                code => OnFlightConfirmationDialogClosed(destination, code)));
        }

        private void OnFlightConfirmationDialogClosed(int destination, WindowExitCode code)
        {
            if (code == WindowExitCode.Ok)
                StateMachine.LoadState(StateFactory.CreateTravelState(destination));
        }

		private void OnStartBattle(ICombatModel combatModel, System.Action<ICombatModel> onCompletedAction)
        {
            if (!IsActive)
                throw new BadGameStateException();

			StateMachine.LoadAdditionalState(StateFactory.CreateCombatState(combatModel, onCompletedAction));
        }

        private void OnStartExploration(Planet planet)
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateExplorationState(planet));
        }

        private void OnQuestActionRequired()
		{
			if (!IsActive)
				return;
			
			UpdateQuests();
		}

        private void OnEscapePressed()
        {
            if (!IsActive)
                return;

            if (_motherShip.ViewMode != ViewMode.StarMap)
                _motherShip.ViewMode = ViewMode.StarMap;
            else
                OnExit();
        }

        private void OnExit()
        {
            if (!IsActive)
                return;
            
            StateMachine.UnloadActiveState();
        }

        private void OnOpenSkillTree()
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateSkillTreeState());
        }

        private void OnOpenConstructor(IShip ship)
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateConstructorState(ship));
            DesiredWindowOnResume = Gui.StarMap.WindowNames.HangarWindow;
        }

        private void OnOpenShop(IInventory marketInventory, IInventory playerInventory)
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateDialogState(Gui.StarMap.WindowNames.MarketDialog, new WindowArgs(marketInventory, playerInventory)));
        }

        private void OnOpenWorkshop()
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateDialogState(Gui.StarMap.WindowNames.WorkshopDialog, new WindowArgs()));
        }

        private void OnOpenShipyard(Faction faction, int level)
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateDialogState(Gui.StarMap.WindowNames.ShipyardWindow, new WindowArgs(faction, level)));
        }

        public void OnOpenEhopedia()
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadAdditionalState(StateFactory.CreateEchopediaState());
        }

        private bool CheckStarGuardian()
		{
			if (!IsActive)
				return false;

			var star = _session.StarMap.PlayerPosition;
			var guardian = _starData.GetOccupant(star);

		    if (guardian.IsAggressive)
		    {
		        OptimizedDebug.Log("Attacked by occupants");
		        guardian.Attack();
                return true;
            }

            return false;
        }

		private void UpdateQuests()
		{
		    if (!IsActive)
		        return;

		    if (_questManager.ActionRequired)
		        _questManager.InvokeAction(this);
		}

        public void ShowUiDialog(IUserInteraction userInteraction)
        {
            StateMachine.LoadAdditionalState(StateFactory.CreateQuestState(userInteraction));
        }

        public void Retreat()
        {
            OnRetreat();
        }

        public void SetCharacterRelations(int characterId, int value, bool additive)
        {
            _session.Quests.SetCharacterRelations(characterId, additive ? value + _session.Quests.GetCharacterRelations(characterId) : value);
        }

        public void SetFactionRelations(int starId, int value, bool additive)
        {
            _session.Quests.SetFactionRelations(starId, additive ? value + _session.Quests.GetFactionRelations(starId) : value);
        }

        public void StartQuest(QuestModel quest)
        {
            _questManager.StartQuest(quest);
        }

        public void OpenShipyard(Faction faction, int level)
        {
            OnOpenShipyard(faction, level);
        }

        public void CaptureStarBase(int starId, bool capture)
        {
            _starData.GetRegion(starId).IsCaptured = capture;
        }

        public void ChangeFaction(int starId, Faction faction)
        {
            _starData.GetRegion(starId).Faction = faction;
        }

        public void SuppressOccupant(int starId, bool destroy)
        {
            _starData.GetOccupant(starId).Suppress(destroy);
        }

        public void StartCombat(ICombatModel model)
        {
            StateMachine.LoadAdditionalState(StateFactory.CreateCombatState(model, value => _questEventTrigger.Fire(new CombatEventData(value))));
        }

        public void StartTrading(ILoot merchantItems)
        {
            OnOpenShop(_inventoryFactory.CreateQuestInventory(merchantItems), _inventoryFactory.CreatePlayerInventory());
        }

        private void OnSupplyShipActivated(bool active)
        {
            _supplyShipStatusChanged = true;
            ShowOutOfFuelDialog();
        }

        private void ShowOutOfFuelDialog()
        {
            if (!IsActive)
                return;

            if (_supplyShipStatusChanged)
            {
                _supplyShipStatusChanged = false;
                if (_supplyShip.IsActive)
                    Observable.EveryUpdate().Skip(3).First().Subscribe(_ => _guiManager.OpenWindow(Gui.Common.WindowNames.OutOfFuelDialog));
            }
        }

        private bool _supplyShipStatusChanged = true;

        private readonly IQuestManager _questManager;
		private readonly ISessionData _session;
		private readonly StarData _starData;
        private readonly MotherShip _motherShip;
        private readonly PlayerResources _playerResources;
        private readonly InventoryFactory _inventoryFactory;
        private readonly IGuiManager _guiManager;
        private readonly RetreatSignal _retreatSignal;
        private readonly StartTravelSignal _startTravelSignal;
        private readonly StartBattleSignal _startBattleSignal;
		private readonly QuestActionRequiredSignal _questActionRequiredSignal;
        private readonly QuestEventSignal.Trigger _questEventTrigger;
        private readonly OpenSkillTreeSignal _openSkillTreeSignal;
        private readonly OpenConstructorSignal _openConstructorSignal;
        private readonly OpenShopSignal _openShopSignal;
        private readonly OpenWorkshopSignal _openWorkshopSignal;
        private readonly OpenShipyardSignal _openShipyardSignal;
        private readonly OpenEhopediaSignal _openEhopediaSignal;
        private readonly ExitSignal _exitSignal;
        private readonly SupplyShip _supplyShip;
        private readonly EscapeKeyPressedSignal _escapeKeyPressedSignal;
        private readonly PlayerPositionChangedSignal _playerPositionChangedSignal;
        private readonly StartExplorationSignal _startExplorationSignal;
        private readonly SupplyShipActivatedSignal _supplyShipActivatedSignal;

        private string DesiredWindowOnResume { get; set; }

        public class Factory : Factory<StarMapState> { }
    }

    public class RetreatSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

	public class StartTravelSignal : SmartWeakSignal<int>
	{
		public class Trigger : TriggerBase { }
	}

    public class StartBattleSignal : SmartWeakSignal<ICombatModel, System.Action<ICombatModel>>
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenSkillTreeSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenConstructorSignal : SmartWeakSignal<IShip>
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenShopSignal : SmartWeakSignal<IInventory, IInventory>
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenWorkshopSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenShipyardSignal : SmartWeakSignal<Faction, int>
    {
        public class Trigger : TriggerBase { }
    }

    public class StartExplorationSignal : SmartWeakSignal<Planet>
    {
        public class Trigger : TriggerBase { }
    }

    public class OpenEhopediaSignal : SmartWeakSignal
    {
        public class Trigger : TriggerBase { }
    }
}
