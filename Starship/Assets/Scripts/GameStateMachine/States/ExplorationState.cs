using System;
using Game.Exploration;
using GameServices.LevelManager;
using Gui.Combat;
using Services.Gui;
using Zenject;

namespace GameStateMachine.States
{
    public class ExplorationState : BaseState
    {
        [Inject]
        public ExplorationState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            IGuiManager guiManager,
            Planet planet,
            EscapeKeyPressedSignal escapeKeyPressedSignal,
            ExitSignal exitSignal)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _guiManager = guiManager;
            _planet = planet;
            _exitSignal = exitSignal;
            _exitSignal.Event += OnCombatCompleted;
            _escapeKeyPressedSignal = escapeKeyPressedSignal;
            _escapeKeyPressedSignal.Event += OnEscapeKeyPressed;
        }

        public override StateType Type => StateType.Exploration;

        protected override Action<DiContainer> Installer => InstallBindings;
        protected override LevelName RequiredLevel => LevelName.Exploration;

        private void OnEscapeKeyPressed()
        {
            _guiManager.OpenWindow(WindowNames.CombatMenuWindow);
        }

        private void OnCombatCompleted()
        {
            //if (!IsActive)
            //    return;

            //var reward = _combatModel.GetReward(_lootGenerator, _playerSkills, _motherShip.CurrentStar);
            //reward.Consume(_playerSkills);

            //var action = _onCompleteAction;
            //_onCompleteAction = null;

            //if (action != null)
            //    action.Invoke(_combatModel);

            //_combatCompletedTrigger.Fire(_combatModel);

            //if (reward.Any())
            //    ShowRewardDialog(reward);

            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        //private void ShowRewardDialog(IReward reward)
        //{
        //    StateMachine.LoadState(StateFactory.CreateCombatRewardState(reward));
        //}

        private void InstallBindings(DiContainer container)
        {
            container.Bind<Planet>().FromInstance(_planet);
        }

        private readonly Planet _planet;
        private readonly IGuiManager _guiManager;
        private readonly EscapeKeyPressedSignal _escapeKeyPressedSignal;
        private readonly ExitSignal _exitSignal;

        public class Factory : Factory<Planet, ExplorationState> { }
    }
}
