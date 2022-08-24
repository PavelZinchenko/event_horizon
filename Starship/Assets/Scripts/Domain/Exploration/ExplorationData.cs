using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using Economy.Products;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Extensions;
using GameDatabase.Model;
using GameServices.Economy;
using Session;
using UnityEngine;
using Zenject;

namespace Game.Exploration
{
    public class ExplorationData
    {
        public enum EnvironmentObjectType
        {
            None,
            SmallCrater,
            BigCrater,
            GasCloud,
        }

        [Inject]
        public ExplorationData(IDatabase database, ISessionData session, LootGenerator lootGenerator, Planet planet)
        {
            _lootGenerator = lootGenerator;
            _database = database;
            _session = session;
            _planet = planet;

            _planet.OnExplorationStarted();
        }

        public int Seed => _planet.Seed;
        public Color PlanetColor => _planet.Color;
        public bool HasSolidGround => _planet.Type != PlanetType.Gas;

        public EnvironmentObjectType GetEnvironmentObject(System.Random random)
        {
            if (_planet.Type == PlanetType.Barren)
                return random.Percentage(80) ? EnvironmentObjectType.SmallCrater : EnvironmentObjectType.BigCrater;
            if (_planet.Type == PlanetType.Gas)
                return EnvironmentObjectType.GasCloud;
            if (_planet.Type == PlanetType.Infected)
                return random.Percentage(80) ? EnvironmentObjectType.GasCloud : random.Percentage(50) ? EnvironmentObjectType.SmallCrater : EnvironmentObjectType.BigCrater;
            
            return EnvironmentObjectType.None;
        }

        public Color GetGasCloudColor(int id)
        {
            var random = new System.Random(Seed + id);

            var value0 = 0.5f + 0.5f * random.NextFloat();
            var value1 = 0.25f + 0.25f * random.NextFloat();
            var value2 = 0.25f + 0.25f * random.NextFloat();

            switch (Seed % 3)
            {
                case 0: return new Color(value0, value1, value2, 0.5f);
                case 1:
                case -1: return new Color(value1, value0, value2, 0.5f);
                default: return new Color(value1, value2, value0, 0.5f);
            }
        }

        public bool IsCompleted(ObjectiveInfo objective)
        {
            return ((_session.StarMap.GetPlanetData(_planet.StarId, _planet.Index) >> objective.Id) & 1) == 1;
        }

        public void Complete(int objectiveId)
        {
            var value = _session.StarMap.GetPlanetData(_planet.StarId, _planet.Index) | (1u << objectiveId);
            _session.StarMap.SetPlanetData(_planet.StarId, _planet.Index, value);
        }

        public IList<ObjectiveInfo> Objectives
        {
            get
            {
                if (_objectives.Count > 0)
                    return _objectives.AsReadOnly();

                var random = new System.Random(Seed);
                var count = _planet.TotalObjectives;

                for (var i = 0; i < count; ++i)
                {
                    switch (_planet.Type)
                    {
                        case PlanetType.Gas: AddObjective(CreateGasPlanetObjective(random, i)); break;
                        case PlanetType.Barren: AddObjective(CreateBarrenPlanetObjective(random, i)); break;
                        case PlanetType.Terran: AddObjective(CreateTerranPlanetObjective(random, i)); break;
                        case PlanetType.Infected: AddObjective(CreateInfectedPlanetObjective(random, i)); break;
                        default: throw new ArgumentException("Invalid planet type: " + _planet.Type);
                    }
                }

                return _objectives.AsReadOnly();
            }
        }

        private ObjectiveType CreateGasPlanetObjective(System.Random random, int id)
        {
            var value = random.Next(100);
            if (value < 30) return ObjectiveType.Meteorite;
            if (value < 40) return ObjectiveType.MineralsRare;
            if (value < 50) return ObjectiveType.ShipWreck;
            if (value < 60) return ObjectiveType.Container;
            return ObjectiveType.Minerals;
        }

        private ObjectiveType CreateBarrenPlanetObjective(System.Random random, int id)
        {
            var value = random.Next(100);
            if (value < 30) return ObjectiveType.Meteorite;
            if (value < 40) return ObjectiveType.MineralsRare;
            if (value < 60) return ObjectiveType.ShipWreck;
            if (value < 70) return ObjectiveType.Container;
            if (value < 72) return ObjectiveType.Outpost;
            return ObjectiveType.Minerals;
        }

        private ObjectiveType CreateTerranPlanetObjective(System.Random random, int id)
        {
            var haveOutpost = _planet.Level > 10;

            if (id == 0 && haveOutpost) return ObjectiveType.Outpost;

            var value = random.Next(100);
            if (value < 40) return ObjectiveType.Meteorite;
            if (value < 45) return ObjectiveType.MineralsRare;
            if (value < 60) return ObjectiveType.Minerals;
            if (value < 75) return ObjectiveType.ShipWreck;
            if (value < 95) return ObjectiveType.Container;
            return haveOutpost ? ObjectiveType.Outpost : ObjectiveType.Container;
        }

        private ObjectiveType CreateInfectedPlanetObjective(System.Random random, int id)
        {
            if (id == 0) return ObjectiveType.Hive;

            var value = random.Next(100);
            if (value < 40) return ObjectiveType.Meteorite;
            if (value < 45) return ObjectiveType.MineralsRare;
            if (value < 60) return ObjectiveType.Minerals;
            if (value < 75) return ObjectiveType.ShipWreck;
            if (value < 95) return ObjectiveType.Container;
            return ObjectiveType.Hive;
        }

        private void AddObjective(ObjectiveType type)
        {
            var id = _objectives.Count;
            _objectives.Add(new ObjectiveInfo(type, Seed + id, id));
        }

        public IEnumerable<IEnemyShipBuilder> EnemyShips
        {
            get
            {
                var count = Mathf.Clamp(_planet.Level/2, 10, 100);
                var random = new System.Random(Seed + 10000);

                for (var i = 0; i < count; ++i)
                {
                    var id = new ItemId<ShipBuild>(GetRandomShip(random));
                    yield return new EnemyShipBuilder(id, _database, _planet.Level, Seed + i);
                }
            }
        }

        public Ship GetShipWreck(int seed)
        {
            return _database.ShipList.Where(IsValidShipForWreck).RandomElement(new System.Random(seed));
        }

        public IEnemyShipBuilder GetOutpost(int seed)
        {
            return new RandomOutpostBuilder(_database, _planet.Level, seed);
        }

        public IEnemyShipBuilder GetHive(int seed)
        {
            return new EnemyShipBuilder(new ItemId<ShipBuild>(275), _database, _planet.Level, seed, true);
        }

        public IEnemyShipBuilder GetHiveGuardian(System.Random random)
        {
            var value = random.Next2(Mathf.Min(140, 60 + _planet.Level / 2));
            var ship = value < 50 ? SmallShips[random.Next(SmallShips.Count)] : BigShips[random.Next(BigShips.Count)];
            return new EnemyShipBuilder(new ItemId<ShipBuild>(ship), _database, _planet.Level, random.Next());
        }

        public IEnemyShipBuilder GetTurret(int seed)
        {
            return new RandomTurretBuilder(_database, _planet.Level, seed);
        }

        public IEnumerable<IProduct> GetLoot(ObjectiveInfo objective)
        {
            switch (objective.Type)
            {
                case ObjectiveType.Container:
                    return _lootGenerator.GetContainerLoot(_planet.Faction, _planet.Level, objective.Seed);
                case ObjectiveType.ShipWreck:
                    return _lootGenerator.GetShipWreckLoot(_planet.Faction, _planet.Level, objective.Seed);
                case ObjectiveType.Outpost:
                    return _lootGenerator.GetOutpostLoot(_planet.Faction, _planet.Level, objective.Seed);
                case ObjectiveType.Hive:
                    return _lootGenerator.GetHiveLoot(_planet.Level, objective.Seed);
                case ObjectiveType.Meteorite:
                    return _lootGenerator.GetMeteoriteLoot(_planet.Faction, _planet.Level, objective.Seed);
                case ObjectiveType.Minerals:
                    return _lootGenerator.GetPlanetResources(_planet.Type, _planet.Faction, _planet.Level, objective.Seed);
                case ObjectiveType.MineralsRare:
                    return _lootGenerator.GetPlanetRareResources(_planet.Type, _planet.Faction, _planet.Level, objective.Seed);
                default:
                    throw new ArgumentOutOfRangeException("Wrong objective type: " + objective.Type);
            }
        }

        private bool IsValidShipForWreck(Ship ship)
        {
            var category = ship.ShipCategory;
            if (category != ShipCategory.Common && category != ShipCategory.Flagship && category != ShipCategory.Rare)
                return false;

            return true;
        }

        private int GetRandomShip(System.Random random)
        {
            var value = random.Next2(Mathf.Min(140, 60 + _planet.Level/2));
            if (value < 50)
                return Drones[random.Next(Drones.Count)];
            if (value < 90)
                return SmallShips[random.Next(SmallShips.Count)];
            if (value < 100)
                return BigShips[random.Next(BigShips.Count)];

            return Flagships[random.Next(Flagships.Count)];
        }

        private bool IsValidFaction(ShipBuild build)
        {
            if (_planet.Type == PlanetType.Infected)
                return true;

            var shipFaction = build.Ship.Faction;
            if (shipFaction == Faction.Neutral || shipFaction == Faction.Undefined)
                return true;
            if (_planet.Faction == Faction.Neutral)
                return shipFaction.WanderingShipsDistance <= _planet.Level;

            return _planet.Faction == shipFaction;
        }

        private bool IsValidClass(ShipBuild build)
        {
            var maxDifficultyClass = Maths.Distance.MaxShipClass(_planet.Level);
            var minDifficultyClass = Maths.Distance.MinShipClass(_planet.Level);
            return build.DifficultyClass <= maxDifficultyClass && build.DifficultyClass >= minDifficultyClass;
        }

        private static bool IsDrone(ShipBuild build)
        {
            return build.Ship.ShipCategory == ShipCategory.Drone;
        }

        private bool IsAvailable(ShipBuild build)
        {
            if (_planet.Type == PlanetType.Infected)
                return build.Ship.Regeneration;

            return true;
        }

        private static bool IsSmall(ShipBuild build)
        {
            var sizeClass = build.Ship.SizeClass;
            if (sizeClass != SizeClass.Frigate && sizeClass != SizeClass.Destroyer)
                return false;

            var category = build.Ship.ShipCategory;
            if (category != ShipCategory.Common && category != ShipCategory.Rare)
                return false;

            return true;
        }

        private static bool IsBig(ShipBuild build)
        {
            var sizeClass = build.Ship.SizeClass;
            if (sizeClass != SizeClass.Cruiser && sizeClass != SizeClass.Battleship)
                return false;

            var category = build.Ship.ShipCategory;
            if (category != ShipCategory.Common && category != ShipCategory.Rare)
                return false;

            return true;
        }

        private static bool IsFlagship(ShipBuild build)
        {
            if (build.Ship.SizeClass != SizeClass.Titan)
                return false;

            var category = build.Ship.ShipCategory;
            if (category == ShipCategory.Starbase || category == ShipCategory.Hidden)
                return false;

            return true;
        }

        private List<int> Drones
        {
            get
            {
                if (_drones == null)
                    _drones = new List<int>(_database.ShipBuildList.Available().Where(IsDrone).Where(IsAvailable).Select(build => build.Id.Value));

                if (_drones.Count == 0) _drones.AddRange(_database.ShipBuildList.Where(IsDrone).Select(build => build.Id.Value));
                return _drones;
            }
        }

        private List<int> Flagships
        {
            get
            {
                if (_capitalShips == null)
                    _capitalShips = new List<int>(_database.ShipBuildList.Available().Where(IsFlagship).Where(IsAvailable).
                        Where(IsValidFaction).Where(IsValidClass).Select(build => build.Id.Value));

                if (_capitalShips.Count == 0) _capitalShips.AddRange(_database.ShipBuildList.Where(IsFlagship).Select(build => build.Id.Value));
                return _capitalShips;
            }
        }

        private List<int> SmallShips
        {
            get
            {
                if (_smallShips == null)
                    _smallShips = new List<int>(_database.ShipBuildList.Available().Where(IsSmall).Where(IsAvailable)
                        .Where(IsValidFaction).Where(IsValidClass).Select(build => build.Id.Value));

                if (_smallShips.Count == 0) _smallShips.AddRange(_database.ShipBuildList.Where(IsSmall).Select(build => build.Id.Value));
                return _smallShips;
            }
        }

        private List<int> BigShips
        {
            get
            {
                if (_bigShips == null)
                    _bigShips = new List<int>(_database.ShipBuildList.Available().Where(IsBig).Where(IsAvailable)
                        .Where(IsValidFaction).Where(IsValidClass).Select(build => build.Id.Value));

                if (_bigShips.Count == 0) _bigShips.AddRange(_database.ShipBuildList.Where(IsBig).Select(build => build.Id.Value));
                return _bigShips;
            }
        }

        private List<int> _drones;
        private List<int> _smallShips;
        private List<int> _bigShips;
        private List<int> _capitalShips;

        private readonly Planet _planet;
        private readonly ISessionData _session;
        private readonly List<ObjectiveInfo> _objectives = new List<ObjectiveInfo>();
        private readonly IDatabase _database;
        private readonly LootGenerator _lootGenerator;
    }

    public enum ObjectiveType
    {
        Container,
        ShipWreck,
        Outpost,
        Meteorite,
        Minerals,
        MineralsRare,
        Hive,
        // TODO: XmasBox,
    }

    public struct ObjectiveInfo
    {
        public ObjectiveInfo(ObjectiveType type, int seed, int id)
        {
            Type = type;
            Seed = seed;
            Id = id;
        }

        public readonly ObjectiveType Type;
        public readonly int Seed;
        public readonly int Id;
    }
}
