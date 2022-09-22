using System;
using System.Collections.Generic;
using System.Linq;
using Combat.Component.Unit.Classification;
using Combat.Domain;
using Constructor.Ships;
using Economy;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.Enums;
using GameServices.Economy;
using GameServices.Random;
using GameStateMachine.States;
using GameDatabase.DataModel;
using GameDatabase.Extensions;
using GameDatabase.Model;
using Model.Factories;
using Session;
using UnityEngine;
using Utils;
using Zenject;

namespace Galaxy.StarContent
{
    public class Challenge
    {
        [Inject] private readonly ISessionData _session;
        [Inject] private readonly IRandom _random;
        [Inject] private readonly ItemTypeFactory _itemTypeFactory;
        [Inject] private readonly StarData _starData;
        [Inject] private readonly StarContentChangedSignal.Trigger _starContentChangedTrigger;
        [Inject] private readonly StartBattleSignal.Trigger _startBattleTrigger;
        [Inject] private readonly LootGenerator _lootGenerator;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly CombatModelBuilder.Factory _combatModelBuilderFactory;

        public bool IsCompleted(int starId) { return GetCurrentLevel(starId) >= MaxLevel; }
        public int GetCurrentLevel(int starId) { return _session.CommonObjects.GetIntValue(starId); }
        public int MaxLevel { get { return 5; } }

        public ShipBuild GetEnemyShip(int starId)
        {
            var level = _starData.GetLevel(starId);
            var ships = _database.ShipBuildList.
                Available().
                NormalShips().
                LimitLevel(level).
                OfClass(DifficultyClass.Default, DifficultyClass.Default).
                OfFaction(Faction.Undefined).
                RandomUniqueElements(MaxLevel, new System.Random(starId)).
                Take(MaxLevel).
                ToList();

            ships.Sort((first, second) => first.Ship.Layout.CellCount - second.Ship.Layout.CellCount);

            var stage = GetCurrentLevel(starId);
            if (ships.Count <= stage)
            {
                OptimizedDebug.LogException(new InvalidOperationException("Challenge: no more ships - " + stage));
                return _database.GetShipBuild(new ItemId<ShipBuild>(DefaultShipBuild));
            }

            return ships[stage];
        }

        public ShipBuild GetPlayerShip(int starId)
        {
            var level = _starData.GetLevel(starId);
            var ship = _database.ShipBuildList.Available().Where(item =>
                item.DifficultyClass == DifficultyClass.Default &&
                item.Ship.ShipCategory == ShipCategory.Common &&
                item.Ship.Faction.WanderingShipsDistance <= level &&
                item.Ship.Layout.CellCount <= 45).
                RandomElement(new System.Random(starId));

            return ship ?? _database.GalaxySettings.StartingShipBuilds.FirstOrDefault() ?? _database.GetShipBuild(new ItemId<ShipBuild>(DefaultShipBuild));
        }

        public void Attack(int starId)
        {
            if (IsCompleted(starId))
                throw new System.InvalidOperationException();

            var level = _starData.GetLevel(starId);

            var playerFleet = new Model.Military.SingleShip(new CommonShip(GetPlayerShip(starId)));

            var ailevel = 104; // TODO
            var enemyFleet = new Model.Military.SingleShip(new EnemyShip(GetEnemyShip(starId)), ailevel);

            var builder = _combatModelBuilderFactory.Create();
            builder.PlayerFleet = playerFleet;
            builder.EnemyFleet = enemyFleet;
            builder.Rules = CombatRules.Challenge();
            builder.AddSpecialReward(GetReward(starId));

            _startBattleTrigger.Fire(builder.Build(), result => OnCombatCompleted(starId, result));
        }

        private IEnumerable<IProduct> GetReward(int starId)
        {
            var step = GetCurrentLevel(starId);
            var level = _starData.GetLevel(starId);

            yield return new Product(_lootGenerator.GetRandomComponent(level + (step + 1) * 10, Faction.Undefined, starId + step + 3456, false));

            if (step + 1 < MaxLevel)
                yield break;

            var random = _random.CreateRandom(starId + 98765);
            yield return Price.Premium(random.Next(1, 4)).GetProduct(_itemTypeFactory);
        }

        private void OnCombatCompleted(int starId, ICombatModel result)
        {
            if (result.GetWinner() != UnitSide.Player)
                return;

            _session.CommonObjects.SetIntValue(starId, GetCurrentLevel(starId) + 1);
            _starContentChangedTrigger.Fire(starId);
        }

        public struct Facade
        {
            public Facade(Challenge challenge, int starId)
            {
                _challenge = challenge;
                _starId = starId;
            }

            public ShipBuild GetEnemyShip() { return _challenge.GetEnemyShip(_starId); }
            public ShipBuild GetPlayerShip() { return _challenge.GetPlayerShip(_starId); }
            public bool IsCompleted { get { return _challenge.IsCompleted(_starId); } }
            public int CurrentLevel { get { return _challenge.GetCurrentLevel(_starId); } }
            public int MaxLevel { get { return _challenge.MaxLevel; } }
            public void Attack() { _challenge.Attack(_starId); }

            private readonly Challenge _challenge;
            private readonly int _starId;
        }

        private const int DefaultShipBuild = 39;
    }
}
