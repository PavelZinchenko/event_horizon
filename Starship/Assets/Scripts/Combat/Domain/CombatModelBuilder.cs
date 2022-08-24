using System.Collections.Generic;
using System.Linq;
using Combat.Component.Unit.Classification;
using Combat.Scene;
using Domain.Quests;
using Economy.Products;
using GameDatabase;
using GameServices.Player;
using Model.Military;
using Zenject;

namespace Combat.Domain
{
    public interface ICombatModelBuilder
    {
        ICombatModel Build(IEnumerable<IProduct> specialLoot = null);
        IFleet PlayerFleet { get; }
        IFleet EnemyFleet { get; }
    }

    public class CombatModelBuilder : ICombatModelBuilder
    {
        public CombatModelBuilder(IDatabase database, ShipCreatedSignal shipCreatedSignal, ShipDestroyedSignal shipDestroyedSignal, PlayerSkills playerSkills)
        {
            _database = database;
            _shipDestroyedSignal = shipDestroyedSignal;
            _shipCreatedSignal = shipCreatedSignal;
            _playerSkills = playerSkills;

            Rules = Model.Factories.CombatRules.Default();
        }

        public IFleet EnemyFleet { get; set; }
        public IFleet PlayerFleet { get; set; }

        public CombatRules Rules { get; set; }

        public void AddSpecialReward(IProduct item)
        {
            _specialReward.Add(item);
        }

        public void AddSpecialReward(IEnumerable<IProduct> items)
        {
            _specialReward.AddRange(items);
        }

        public ICombatModel Build(IEnumerable<IProduct> specialLoot = null)
        {
            var playerFleet = PlayerFleet ?? Model.Factories.Fleet.Empty;
            var enemyFleet = EnemyFleet ?? Model.Factories.Fleet.Empty;
            var useBonuses = !Rules.DisableBonusses;

            var model = new CombatModel(
                new FleetModel(playerFleet.Ships, UnitSide.Player, _database, playerFleet.AiLevel, useBonuses ? _playerSkills : null),
                new FleetModel(enemyFleet.Ships, UnitSide.Enemy, _database, enemyFleet.AiLevel),
                _shipCreatedSignal, 
                _shipDestroyedSignal);

            model.SpecialRewards = specialLoot != null ? _specialReward.Concat(specialLoot) : _specialReward;
            model.Rules = Rules;

            return model;
        }

        private readonly IDatabase _database;
        private readonly ShipCreatedSignal _shipCreatedSignal;
        private readonly ShipDestroyedSignal _shipDestroyedSignal;
        private readonly List<IProduct> _specialReward = new List<IProduct>();
        private readonly PlayerSkills _playerSkills;

        public class Factory : Factory<CombatModelBuilder> { }
    }
}
