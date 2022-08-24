using GameServices.LevelManager;
using Scripts.GameStateMachine;
using Services.Gui;
using Zenject;

namespace GameStateMachine.States
{
    class DialogState : BaseState
    {
        [Inject]
        public DialogState(
            string windowName,
            WindowArgs windowArgs,
            System.Action<WindowExitCode> onExitAction,
            IStateMachine stateMachine,
            ILevelLoader levelLoader,
            GameStateFactory stateFactory,
            IGuiManager guiManager)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _windowName = windowName;
            _windowArgs = windowArgs;
            _guiManager = guiManager;
            _onExitAction = onExitAction;
        }

        public override StateType Type { get { return StateType.Dialog; } }

        protected override LevelName RequiredLevel { get { return LevelName.StarMap; } }

        protected override void OnActivate()
        {
            _guiManager.OpenWindow(_windowName, _windowArgs, OnDialogClosed);
        }

        protected virtual void OnExit(WindowExitCode exitCode)
        {
            StateMachine.UnloadActiveState();
        }

        private void OnDialogClosed(WindowExitCode exitCode)
        {
            if (!IsActive)
                throw new BadGameStateException();

            var action = _onExitAction;
            _onExitAction = null;
            if (action != null)
                action.Invoke(exitCode);

            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private readonly string _windowName;
        private readonly WindowArgs _windowArgs;
        private readonly IGuiManager _guiManager;
        private System.Action<WindowExitCode> _onExitAction;

        public class Factory : Factory<string, WindowArgs, System.Action<WindowExitCode>, DialogState> { }
    }
}
