using System;
using Combat.Domain;
using GameModel.Quests;
using GameServices.LevelManager;
using GameServices.Player;
using Utils;
using Zenject;

namespace GameStateMachine.States
{
    public class CombatState : BaseState
    {
        [Inject]
        public CombatState(
            IStateMachine stateMachine,
            GameStateFactory stateFactory,
            ILevelLoader levelLoader,
            ICombatModel combatModel,
            System.Action<ICombatModel> onCompleteAction,
            PlayerSkills playerSkills,
            MotherShip motherShip,
            GameServices.Economy.LootGenerator lootGenerator,
            ExitSignal exitSignal,
            CombatCompletedSignal.Trigger combatCompletedTrigger)
            : base(stateMachine, stateFactory, levelLoader)
        {
            _combatModel = combatModel;
            _motherShip = motherShip;
            _lootGenerator = lootGenerator;
            _combatCompletedTrigger = combatCompletedTrigger;
            _playerSkills = playerSkills;
            _onCompleteAction = onCompleteAction;

            _exitSignal = exitSignal;
            _exitSignal.Event += OnCombatCompleted;
        }

        public override StateType Type => StateType.Combat;

        protected override Action<DiContainer> Installer => InstallBindings;
        protected override LevelName RequiredLevel => LevelName.Combat;

        private void OnCombatCompleted()
        {
            if (!IsActive)
                return;

            var reward = _combatModel.GetReward(_lootGenerator, _playerSkills, _motherShip.CurrentStar);
            reward.Consume(_playerSkills);

            var action = _onCompleteAction;
            _onCompleteAction = null;

            if (action != null)
                action.Invoke(_combatModel);

            _combatCompletedTrigger.Fire(_combatModel);

            if (reward.Any())
                ShowRewardDialog(reward);

            if (IsActive)
                StateMachine.UnloadActiveState();
        }

        private void ShowRewardDialog(IReward reward)
        {
            StateMachine.LoadState(StateFactory.CreateCombatRewardState(reward));
        }
        
        private void InstallBindings(DiContainer container)
        {
            container.Bind<ICombatModel>().FromInstance(_combatModel);
        }

        private System.Action<ICombatModel> _onCompleteAction;
        private readonly ICombatModel _combatModel;
        private readonly ExitSignal _exitSignal;
        private readonly MotherShip _motherShip;
        private readonly GameServices.Economy.LootGenerator _lootGenerator;
        private readonly CombatCompletedSignal.Trigger _combatCompletedTrigger;
        private readonly PlayerSkills _playerSkills;

        public class Factory : Factory<ICombatModel, System.Action<ICombatModel>, CombatState> { }
    }

    public class CombatCompletedSignal : SmartWeakSignal<ICombatModel>
    {
        public class Trigger : TriggerBase { }
    }
}
