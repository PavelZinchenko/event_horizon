using System.Linq;
using Combat.Domain;
using Domain.Player;
using Economy.Products;
using GameServices.Gui;
using GameServices.LevelManager;
using Gui.MainMenu;
using Services.Gui;
using Zenject;

namespace GameStateMachine.States
{
    public class DailyRewardState : BaseState
    {
        [Inject]
        public DailyRewardState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            DailyReward dailyReward,
            ExitSignal exitSignal,
            IGuiManager guiManager)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _exitSignal = exitSignal;
            _exitSignal.Event += OnExit;
            _guiManager = guiManager;
            _dailyReward = dailyReward;
        }

        public override StateType Type { get { return StateType.DailyReward; } }

        protected override LevelName RequiredLevel { get { return LevelName.MainMenu; } }

        protected override void OnActivate()
        {
            base.OnActivate();

            var rewards = _dailyReward.CollectReward();
            foreach (var item in rewards)
                item.Consume();

            var args = new WindowArgs(rewards.Cast<object>().ToArray());
            _guiManager.OpenWindow(WindowNames.DailyRewardWindow, args, OnRewardSelecred);
        }

        private void OnRewardSelecred(WindowExitCode exitCode)
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private void OnExit()
        {
            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private readonly IGuiManager _guiManager;
        private readonly ExitSignal _exitSignal;
        private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;
        private readonly DailyReward _dailyReward;

        public class Factory : Factory<DailyRewardState> { }
    }
}
