using GameServices.LevelManager;
using Zenject;

namespace GameStateMachine.States
{
    public class EhopediaState : BaseState
    {
        [Inject]
        public EhopediaState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            ExitSignal exitSignal)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
        }

        public override StateType Type => StateType.Ehopedia;

        protected override LevelName RequiredLevel => LevelName.Ehopedia;

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private readonly ExitSignal _exitSignal;

        public class Factory : Factory<EhopediaState> { }
    }
}
