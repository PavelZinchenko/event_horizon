using System.Linq;
using Constructor.Satellites;
using Constructor.Ships;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using UnityEngine;

namespace Game.Exploration
{
    public class EnemyShipBuilder : IEnemyShipBuilder
    {
        public EnemyShipBuilder(ItemId<ShipBuild> id, IDatabase database, int level, int seed, bool randomizeColor = false, bool allowSatellites = true)
        {
            _database = database;
            _shipId = id.Value;
            _level = level;
            _seed = seed;
            _allowSatellites = allowSatellites;
            _randomizeColor = randomizeColor;
        }

        public Combat.Component.Ship.Ship Build(Combat.Factory.ShipFactory shipFactory, Combat.Factory.SpaceObjectFactory objectFactory, Vector2 position, float rotation)
        {
            var model = CreateShip();
            var spec = model.CreateBuilder().Build(_database.ShipSettings);
            var ship = shipFactory.CreateEnemyShip(spec, position, rotation, Maths.Distance.AiLevel(_level));
            return ship;
        }

        private IShip CreateShip()
        {
            var random = new System.Random(_seed);

            var build = _database.GetShipBuild(new ItemId<ShipBuild>(_shipId));
            var ship = new EnemyShip(build);

            var shipLevel = Maths.Distance.ToShipLevel(_level);
            shipLevel -= random.Next(shipLevel/3);
            ship.Experience = Maths.Experience.FromLevel(shipLevel);

            var satelliteClass = Maths.Distance.MaxShipClass(_level);
            if (_allowSatellites && satelliteClass != DifficultyClass.Default && build.Ship.ShipCategory != ShipCategory.Drone)
            {
                var satellites = _database.SatelliteBuildList.LimitClass(satelliteClass).SuitableFor(build.Ship);
                if (satellites.Any())
                {
                    if (random.Next(3) != 0)
                        ship.FirstSatellite = new CommonSatellite(satellites.RandomElement(random));
                    if (random.Next(3) != 0)
                        ship.SecondSatellite = new CommonSatellite(satellites.RandomElement(random));
                }
            }

            if (_randomizeColor)
            {
                ship.ColorScheme.Type = ShipColorScheme.SchemeType.Hsv;
                ship.ColorScheme.Hue = random.NextFloat();
            }

            return ship;
        }

        private readonly bool _randomizeColor;
        private readonly int _seed;
        private readonly int _shipId;
        private readonly int _level;
        private readonly bool _allowSatellites;
        private readonly IDatabase _database;
    }
}
