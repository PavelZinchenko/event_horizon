using System.Linq;
using Constructor.Ships;
using GameDatabase;
using GameServices.LevelManager;
using Services.Gui;
using UnityEngine;
using Zenject;

namespace GameStateMachine.States
{
    public class EditorInitializationState : BaseState
    {
        [Inject] private readonly IGuiManager _guiManager;
        [Inject] private readonly IDatabase _database;

        [Inject]
        public EditorInitializationState(IStateMachine stateMachine, GameStateFactory stateFactory, ILevelLoader levelLoader)
            : base(stateMachine, stateFactory, levelLoader)
        {
        }

        public override StateType Type { get { return StateType.Initialization; } }

        protected override void OnActivate()
        {
            string error;
            if (!_database.TryLoad(Application.dataPath + "/../../Database/", out error))
            {
                _guiManager.OpenWindow(global::Gui.Notifications.WindowNames.MessageBoxWindow, new WindowArgs("Database loading failure: " + error), result => Application.Quit());
                return;
            }

            var ship = _database.ShipBuildList.First();
            StateMachine.LoadState(StateFactory.CreateConstructorState(new EditorModeShip(ship, _database)));
        }

        protected override LevelName RequiredLevel { get { return LevelName.CommonGui; } }

        protected override void OnSuspend(StateType newState)
        {
        }

        protected override void OnResume(StateType oldState)
        {
        }

        public class Factory : Factory<EditorInitializationState> { }
    }
}
