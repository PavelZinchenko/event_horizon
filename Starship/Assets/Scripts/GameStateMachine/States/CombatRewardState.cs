using GameModel.Quests;
using GameServices.LevelManager;
using Gui.Combat;
using Services.Gui;
using Zenject;

namespace GameStateMachine.States
{
    public class CombatRewardState : BaseState
    {
        [Inject]
        public CombatRewardState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            IGuiManager guiManager,
            IReward reward)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _guiManager = guiManager;
            _reward = reward;
        }

        public override StateType Type
        {
            get { return StateType.Combat; }
        }

        protected override void OnActivate()
        {
            _guiManager.OpenWindow(WindowNames.CombatRewardWindow, new WindowArgs(_reward), OnWindowClosed);
        }

        private void OnWindowClosed(WindowExitCode exitCode)
        {
            StateMachine.UnloadActiveState();
        }

        protected override LevelName RequiredLevel { get { return LevelName.Combat; } }

        private readonly IReward _reward;
        private readonly IGuiManager _guiManager;

        public class Factory : Factory<IReward, CombatRewardState> { }
    }
}
