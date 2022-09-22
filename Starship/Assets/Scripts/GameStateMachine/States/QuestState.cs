using Domain.Quests;
using GameDatabase.Enums;
using GameServices.LevelManager;
using Scripts.GameStateMachine;
using Services.Gui;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
	public class QuestState : BaseState
	{
		[Inject]
		public QuestState(
			IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
			IGuiManager guiManager,
			GameServices.Player.MotherShip motherShip,
			IUserInteraction userInteraction)
			: base(stateMachine, stateFactory, levelLoader)
		{
			_guiManager = guiManager;
			_motherShip = motherShip;
		    _userInteraction = userInteraction;
		}

		public override StateType Type
		{
			get { return StateType.Quest; }
		}

		protected override void OnActivate()
		{
			OptimizedDebug.Log("QuestState loaded");

		    var args = new WindowArgs(_userInteraction);

            switch (_userInteraction.RequiredView)
			{
			case RequiredViewMode.StarSystem:
				_motherShip.ViewMode = GameServices.Player.ViewMode.StarSystem;
				_guiManager.OpenWindow(Gui.Quests.WindowNames.MiniEventDialog, args, OnDialogClosed);
				break;
			case RequiredViewMode.GalaxyMap:
				_motherShip.ViewMode = GameServices.Player.ViewMode.GalaxyMap;
				_guiManager.OpenWindow(Gui.Quests.WindowNames.EventDialog, args, OnDialogClosed);
				break;
			case RequiredViewMode.StarMap:
				_motherShip.ViewMode = GameServices.Player.ViewMode.StarMap;
				_guiManager.OpenWindow(Gui.Quests.WindowNames.EventDialog, args, OnDialogClosed);
				break;
			default:
				_guiManager.OpenWindow(Gui.Quests.WindowNames.EventDialog, args, OnDialogClosed);
				break;
			}
		}

		protected override LevelName RequiredLevel { get { return LevelName.StarMap; } }

		private void OnDialogClosed(WindowExitCode exitCode)
		{
			if (!IsActive)
				throw new BadGameStateException(); 

			StateMachine.UnloadActiveState();
		}

		private readonly IGuiManager _guiManager;
		private readonly IUserInteraction _userInteraction;
		private readonly GameServices.Player.MotherShip _motherShip;

		public class Factory : Factory<IUserInteraction, QuestState> { }
	}
}
