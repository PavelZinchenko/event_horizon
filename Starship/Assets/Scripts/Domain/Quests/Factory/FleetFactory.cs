using System;
using System.Linq;
using Combat.Domain;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameServices.Player;
using Zenject;

namespace Domain.Quests
{
    public class FleetFactory
    {
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;

        public CombatModelBuilder CreateCombatModelBuilder(Fleet enemy, QuestContext context)
        {
            var enemyFleet = CreateFleet(enemy, context);
            var rules = CreateCombatRules(enemy, context);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = Model.Factories.Fleet.Player(_playerFleet, _database);
            builder.EnemyFleet = enemyFleet;
            builder.Rules = rules;

            return builder;
        }

        public Model.Military.IFleet CreateFleet(Fleet enemy, QuestContext context)
        {
            var random = new Random(context.Seed);
            var level = Math.Max(0, context.Level + enemy.LevelBonus);
            var factionFilter = new FactionFilter(enemy.Factions, level, context.Faction);

            var numberOfShips = enemy.NoRandomShips ? 0 : Maths.Distance.FleetSize(level, random);
            var ships = _database.ShipBuildList.Available().Common().Where(item => factionFilter.IsSuitable(item.Ship.Faction)).
                LimitLevelAndClass(level).RandomElements(numberOfShips, random).Concat(enemy.SpecificShips);

            return new Model.Military.QuestFleet(_database, ships.ToList().Shuffle(random), level, random.Next());
        }

        public Model.Military.CombatRules CreateCombatRules(Fleet enemy, QuestContext context)
        {
            var distance = context.Level;

            var rules = CreateDefault(distance);
            if (enemy.CombatTimeLimit > 0) rules.TimeLimit = enemy.CombatTimeLimit;

            rules.LootCondition = enemy.LootCondition;
            rules.ExpCondition = enemy.ExpCondition;

            return rules;
        }

        private Model.Military.CombatRules CreateDefault(int level)
        {
            return new Model.Military.CombatRules
            {
                RewardType = Model.Military.RewardType.Default,
                LootCondition = RewardCondition.Default,
                ExpCondition = RewardCondition.Default,
                TimeLimit = Maths.Distance.CombatTime(level),
                TimeoutBehaviour = Model.Military.TimeoutBehaviour.NextEnemy,
                CanSelectShips = true,
                CanCallEnemies = true,
                AsteroidsEnabled = true,
                PlanetEnabled = true,
                InitialEnemies = 1,
                MaxEnemies = 3
            };
        }
    }
}
