using Combat.Component.Unit.Classification;
using Combat.Domain;
using GameDatabase;
using GameServices.Economy;
using GameServices.Player;
using GameServices.Random;
using GameStateMachine.States;
using Model.Factories;
using Session;
using Zenject;

namespace Galaxy.StarContent
{
    public class Ruins
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;
        [Inject] private readonly LootGenerator _lootGenerator;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;

        public bool IsDefeated(int starId)
        {
            return _session.Bosses.DefeatCount(starId) > 0;
        }

        public Model.Military.IFleet CreateFleet(int starId)
        {
            return Fleet.Ruins(_starData.GetLevel(starId), starId + _random.Seed, _database);
        }

        public void Attack(int starId)
        {
            if (IsDefeated(starId))
                throw new System.InvalidOperationException();

            var level = _starData.GetLevel(starId);
            var firstFleet = new Model.Military.PlayerFleet(_database, _playerFleet);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = firstFleet;
            builder.EnemyFleet = CreateFleet(starId);
            builder.Rules = CombatRules.Flagship(level);
            builder.AddSpecialReward(_lootGenerator.GetRuinsRewards(level, starId));

            _startBattleTrigger.Fire(builder.Build(), result => OnCombatCompleted(starId, result));
        }

        private void OnCombatCompleted(int starId, ICombatModel result)
        {
            if (result.GetWinner() != UnitSide.Player)
                return;

            _session.Bosses.SetCompleted(starId);
            _starContentChangedTrigger.Fire(starId);
        }

        public struct Facade
        {
            public Facade(Ruins ruins, int starId)
            {
                _ruins = ruins;
                _starId = starId;
            }

            public Model.Military.IFleet CreateFleet() { return _ruins.CreateFleet(_starId); }
            public bool IsDefeated { get { return _ruins.IsDefeated(_starId); } }
            public void Attack() { _ruins.Attack(_starId); }

            private readonly Ruins _ruins;
            private readonly int _starId;
        }
    }
}
