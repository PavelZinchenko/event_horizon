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
    public class RandomTurretBuilder : IEnemyShipBuilder
    {
        public RandomTurretBuilder(IDatabase database, int level, int seed)
        {
            _database = database;
            _level = level;
            _seed = seed;
        }

        public Combat.Component.Ship.Ship Build(Combat.Factory.ShipFactory shipFactory, Combat.Factory.SpaceObjectFactory objectFactory, Vector2 position, float rotation)
        {
            var random = new System.Random(_seed);
            var componentLevel = Maths.Distance.ComponentLevel(_level);

            var ship = new CommonShip(new ShipModel(_database.ExplorationSettings.TurretShip), GetComponents(random));
            ship.Experience = Maths.Experience.FromLevel(Maths.Distance.ToShipLevel(_level));
            //ship.ColorScheme.Type = ColorScheme.SchemeType.Hsv;
            //ship.ColorScheme.Hue = random.NextFloat();

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
            var reactorCount = 2;
            for (var i = 0; i < reactorCount; ++i)
                yield return Create(reactor);

            var fuelCells = _database.GetComponent(new ItemId<Component>(LargeFuelCellsId));
            var fuelCellsCount = 4;
            for (var i = 0; i < fuelCellsCount; ++i)
                yield return Create(fuelCells);

            foreach (var item in _database.ComponentList.Available().Where(item => item.Stats.ArmorPoints > 0 && item.Level <= componentLevel).RandomElements(5, random))
                yield return Create(item);

            var weapon = _database.ComponentList.Available().Where(item => IsSuitableWeapon(item, componentLevel)).RandomElement(random) ?? _database.GetComponent(new ItemId<Component>(85)); // default - ProjectileCannon_L
            yield return Create(weapon, 0);
        }

        private static bool IsSuitableWeapon(Component component, int level)
        {
            if (component.Ammunition == null && component.AmmunitionObsolete == null) return false;
            if (component.WeaponSlotType != WeaponSlotType.Cannon) return false;
            if (component.Level > level) return false;
            if (component.Layout.CellCount < 3) return false;

            return true;
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
    }
}
