using GameServices.LevelManager;
using Zenject;

namespace GameStateMachine.States
{
    class SkillTreeState : BaseState
    {
        [Inject]
        public SkillTreeState(
            IStateMachine stateMachine,
            GameStateFactory gameStateFactory,
            ILevelLoader levelLoader,
            ExitSignal exitSignal)
            : base(stateMachine, gameStateFactory, levelLoader)
        {
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
        }

        public override StateType Type { get { return StateType.SkillTree; } }

        protected override LevelName RequiredLevel { get { return LevelName.SkillTree; } }

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private readonly ExitSignal _exitSignal;

        public class Factory : Factory<SkillTreeState> { }
    }
}
