using GameServices.LevelManager;
using Zenject;

namespace GameStateMachine.States
{
    public class TestingState : BaseState
    {
        [Inject]
        public TestingState(
            IStateMachine stateMachine,
            GameStateFactory gameStateFactory,
            ILevelLoader levelLoader,
            ExitSignal exitSignal)
            : base(stateMachine, gameStateFactory, levelLoader)
        {
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
        }

        public override StateType Type
        {
            get { return StateType.Testing; }
        }

        protected override LevelName RequiredLevel { get { return LevelName.ConfigureControls; } }

        protected override void OnLevelLoaded()
        {
        }

        private void OnExit()
        {
            if (!IsActive)
                return;

            StateMachine.UnloadActiveState();
        }

        private readonly ExitSignal _exitSignal;

        public class Factory : Factory<TestingState> { }
    }
}
