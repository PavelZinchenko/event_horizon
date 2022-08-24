using System.Linq;
using Combat.Component.Unit.Classification;
using Combat.Domain;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameModel;
using GameServices.Economy;
using GameServices.Player;
using GameStateMachine.States;
using Model.Factories;
using Services.Messenger;
using Session;
using UnityEngine;
using Zenject;

namespace Galaxy.StarContent
{
    public class StarBase
    {
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;
        [Inject] private readonly ItemTypeFactory _itemTypeFactory;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;
        [Inject] private readonly LootGenerator _lootGenerator;
        [Inject] private readonly ISessionData _session;

        public void Attack(int starId)
        {
            if (!_starData.HasStarBase(starId))
                throw new System.InvalidOperationException();

            var region = _starData.GetRegion(starId);
            if (region.IsCaptured)
                throw new System.InvalidOperationException();

            var playerFleet = new Model.Military.PlayerFleet(_database, _playerFleet);
            var defenderFleet = Fleet.Capital(region, _database);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = playerFleet;
            builder.EnemyFleet = defenderFleet;
            builder.Rules = CombatRules.Capital(region);
            builder.AddSpecialReward(_lootGenerator.GetStarBaseSpecialReward(region));

            _session.Quests.SetFactionRelations(starId, -100);
            _startBattleTrigger.Fire(builder.Build(), result => OnCombatCompleted(starId, result));
        }

        private void OnCombatCompleted(int starId, ICombatModel result)
        {
            if (result.GetWinner() != UnitSide.Player)
                return;

            _starData.GetRegion(starId).IsCaptured = true;
            _starContentChangedTrigger.Fire(starId);
        }
    }
}
