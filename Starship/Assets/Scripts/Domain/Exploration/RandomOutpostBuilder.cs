using System.Collections.Generic;
using System.Linq;
using Combat.Component.Unit.Classification;
using Constructor;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using UnityEngine;
using Component = GameDatabase.DataModel.Component;

namespace Game.Exploration
{
    public class RandomOutpostBuilder : IEnemyShipBuilder
    {
        public RandomOutpostBuilder(IDatabase database, int level, int seed)
        {
            _database = database;
            _level = level;
            _seed = seed;
        }

        public Combat.Component.Ship.Ship Build(Combat.Factory.ShipFactory shipFactory, Combat.Factory.SpaceObjectFactory objectFactory, Vector2 position, float rotation)
        {
            var random = new System.Random(_seed);
            var componentLevel = Maths.Distance.ComponentLevel(_level);

            var ship = new CommonShip(new ShipModel(_database.ExplorationSettings.OutpostShip), GetComponents(random));
            ship.Experience = Maths.Experience.FromLevel(Maths.Distance.ToShipLevel(_level));
            ship.ColorScheme.Type = ShipColorScheme.SchemeType.Hsv;
            ship.ColorScheme.Hue = random.NextFloat();

            var builder = ship.CreateBuilder();
            builder.Converter = new EnemyComponentConverter(componentLevel, random);

            var spec = builder.Build(_database.ShipSettings);
            var starbase = shipFactory.CreateTurret(spec, position, rotation, UnitSide.Enemy, true);

            return starbase;
        }

        private IEnumerable<IntegratedComponent> GetComponents(System.Random random)
        {
            var componentLevel = Maths.Distance.ComponentLevel(_level);

            var reactor = _database.GetComponent(new ItemId<Component>(LargeNuclearReactorId));
            var reactorCount = 8 + random.Next(3);
            for (var i = 0; i < reactorCount; ++i)
                yield return Create(reactor);

            var fuelCells = _database.GetComponent(new ItemId<Component>(LargeFuelCellsId));
            var fuelCellsCount = 6 + random.Next(3);
            for (var i = 0; i < fuelCellsCount; ++i)
                yield return Create(fuelCells);

            var hasShield = random.Percentage(50);
            if (hasShield)
            {
                foreach (var item in _database.ComponentList.Available().Where(item => item.Stats.ShieldPoints > 0).RandomElements(10, random))
                    yield return Create(item);
                foreach (var item in _database.ComponentList.Available().Where(item => item.Stats.ShieldRechargeRate > 0).RandomElements(5, random))
                    yield return Create(item);
            }
            else
            {
                foreach (var item in _database.ComponentList.Available().Where(item =>
                    item.Stats.KineticResistance > 0 ||
                    item.Stats.EnergyResistance > 0 ||
                    item.Stats.ThermalResistance > 0).RandomElements(10, random))
                    yield return Create(item);
            }

            foreach (var item in _database.ComponentList.Available().Where(item => item.Stats.ArmorPoints > 0 && item.Level <= componentLevel).RandomElements(10, random))
                yield return Create(item);

            var dronebayCount = 3;
            var dronebays = _database.ComponentList.Available().Where(item => item.DroneBay != null && item.Level < componentLevel).RandomElements(dronebayCount, random).ToArray();

            foreach (var item in dronebays)
                yield return Create(item);

            if (dronebays.Length == 0)
                yield return Create(_database.ComponentList.Available().Where(item => item.DroneBay != null && item.DroneBay.Stats.Capacity == 1).RandomElement(random));

            yield return Create(_database.GetComponent(new ItemId<Component>(SmallDroneFactoryId)));

            var droneUpgradesCount = 3 + random.Next(3);
            foreach (var item in _database.ComponentList.Available().Where(item => item.DroneBay == null &&
                item.DisplayCategory == ComponentCategory.Drones && item.Level < componentLevel).RandomElements(droneUpgradesCount, random))
                yield return Create(item);
        }

        private static IntegratedComponent Create(Component item, int barrelId = -1)
        {
            return new IntegratedComponent(new ComponentInfo(item), 0, 0, barrelId, 0, 0, false);
        }

        private readonly int _seed;
        private readonly int _level;
        private readonly IDatabase _database;

        private const int LargeNuclearReactorId = 76;
        private const int LargeFuelCellsId = 65;
        private const int SmallDroneFactoryId = 52;
    }
}
