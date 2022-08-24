using GameServices.LevelManager;
using GameServices.Settings;
using Gui.Common;
using Gui.Dialogs;
using Scripts.GameStateMachine;
using Services.Gui;
using Zenject;

namespace GameStateMachine.States
{
    class AnnouncementState : BaseState
    {
        [Inject]
        public AnnouncementState(
            IStateMachine stateMachine,
            ILevelLoader levelLoader,
            GameStateFactory stateFactory,
            IGuiManager guiManager,
            GameSettings gameSettings)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _guiManager = guiManager;
            _gameSettings = gameSettings;
        }

        public override StateType Type { get { return StateType.Dialog; } }

        protected override LevelName RequiredLevel { get { return LevelName.MainMenu; } }

        protected override void OnActivate()
        {
#if UNITY_ANDROID || UNITY_IPHONE
            if (_gameSettings.DontAskAgainId < AnnouncementWindow.AnnouncementId)
                _guiManager.OpenWindow(WindowNames.AnnouncementWindow, OnDialogClosed);
            else
#endif
                OnDialogClosed(WindowExitCode.Cancel);
        }

        private void OnDialogClosed(WindowExitCode exitCode)
        {
            if (!IsActive)
                throw new BadGameStateException();

            StateMachine.LoadState(StateFactory.CreateMainMenuState());
        }

        private readonly IGuiManager _guiManager;
        private readonly GameSettings _gameSettings;

        public class Factory : Factory<AnnouncementState> { }
    }
}
