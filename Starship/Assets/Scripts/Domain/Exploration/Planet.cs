using System.Collections.Generic;
using Galaxy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using GameServices.Player;
using Session;
using GameServices.Random;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Game.Exploration
{
    public enum PlanetType
    {
        Gas,
        Barren,
        Terran,
        Asteroids,
        Infected,
    }

    public static class PlanetTypeExtensions
    {
        public static string Name(this PlanetType type, ILocalization localization)
        {
            return localization.GetString("$PlanetType" + type);
        }
    }

    public class Planet
    {
        [Inject]
        public Planet(Galaxy.Star star, int index, ISessionData session, IRandom randomGenerator, IDatabase database)
        {
            _random = randomGenerator;
            _session = session;

            Index = index;
            StarId = star.Id;
            Seed = index * 13 + star.Id + _random.Seed;
            var random =  _random.CreateRandom(Seed);

            Type = GetPlanetType(index, star.Id, random);
            Icon = GetPlanetImage(Type, random);
            Size = GetPlanetSize(Type, random);
            Level = star.Level;
            Faction = Type == PlanetType.Infected ? database.GetFaction(new ItemId<Faction>(16)) : star.Region.Faction;
            TotalObjectives = random.Range(8,16);
            Color = Type == PlanetType.Infected ? GetInfectedPlanetColor(random) : GetPlanetColor(random);
        }

        private PlanetType GetPlanetType(int index, int starId, System.Random random)
        {
            if (index == InfectedPlanetId) return PlanetType.Infected;

            var firstGasPlanetId = _random.RandomInt(starId, 5);
            if (index >= firstGasPlanetId) return PlanetType.Gas;

            var value = random.Next(100);
            if (value < 20)
                return index > 0 ? PlanetType.Asteroids : PlanetType.Barren;
            if (value < 30)
                return PlanetType.Terran;
            else
                return PlanetType.Barren;
        }

        private static float GetPlanetSize(PlanetType type, System.Random random)
        {
            var size = random.NextFloat();

            if (type == PlanetType.Asteroids) return 1.0f;
            if (type == PlanetType.Infected) return 1.0f;
            if (type == PlanetType.Gas) return 0.6f + size * 0.4f;

            return size * 0.7f;
        }

        private static UnityEngine.Sprite GetPlanetImage(PlanetType type, System.Random random)
        {
            switch (type)
            {
                case PlanetType.Infected:
                    return CommonSpriteTable.InfectedPlanet;
                case PlanetType.Barren:
                    return CommonSpriteTable.BarrenPlanet(random);
                case PlanetType.Gas:
                    return CommonSpriteTable.GasPlanet(random);
                case PlanetType.Terran:
                    return CommonSpriteTable.TerranPlanet(random);
                case PlanetType.Asteroids:
                    return CommonSpriteTable.AsteroidBelt;
                default:
                    throw new System.ArgumentException();
            }
        }

        private static Color GetPlanetColor(System.Random random)
        {
            var p1 = random.NextFloat();
            var p2 = random.NextFloat();
            var p3 = random.NextFloat();
            if (p1 > p2) Generic.Swap(ref p1, ref p2);
            if (p2 > p3) Generic.Swap(ref p2, ref p3);
            if (p1 > p2) Generic.Swap(ref p1, ref p2);

            var temperature = p1;
            var toxicity = p2 - p1;
            var enemies = p3 - p2;

            var r = temperature * 0.8f + 0.2f;
            var g = toxicity * 0.8f + 0.2f;

            return new Color(r + random.NextFloat() * (1f - r), g + random.NextFloat() * (1f - g), 0.5f + random.NextFloat() * 0.5f);
        }

        private static Color GetInfectedPlanetColor(System.Random random)
        {
            var r = random.NextFloat()*0.3f + 0.6f;
            var g = random.NextFloat()*0.2f + 0.8f;
            var b = random.NextFloat()*0.4f + 0.4f;

            return new Color(r, g, b);
        }

        public string Name => Model.Generators.NameGenerator.GetPlanetName(StarId, Index);
        public UnityEngine.Sprite Icon { get; private set; }
        public int Index { get; private set; }
        public int StarId { get; private set; }
        public PlanetType Type { get; private set; }
        public float Size { get; private set; }
        public Color Color { get; private set; }
        public int Level { get; private set; }
        public int Seed { get; private set; }
        public Faction Faction { get; private set; }
        public int TotalObjectives { get; private set; }

        public void OnExplorationStarted()
        {
            var data = _session.StarMap.GetPlanetData(StarId, Index);
            data |= 0x80000000;
            _session.StarMap.SetPlanetData(StarId, Index, data);
        }

        public bool WasExplored => _session.StarMap.GetPlanetData(StarId, Index) != 0;

        public int ObjectivesExplored
        {
            get
            {
                var count = 0;
                var value = _session.StarMap.GetPlanetData(StarId, Index);
                for (var i = 0; i < TotalObjectives; ++i)
                {
                    if ((value & 1) == 1) count++;
                    value >>= 1;
                }

                return count;
            }
        }

        //public int NumberOfExplorations
        //{
        //    get { return _session.StarMap.GetPlanetData(StarId, Index); }
        //    private set { _session.StarMap.SetPlanetData(StarId, Index, value); }
        //}

        //public float ExplorationSuccessChance
        //{
        //    get { return 0.75f / Mathf.Pow(1.5f, 1 + NumberOfExplorations) * _playerSkills.PlanetaryScanner; }
        //}

        private readonly ISessionData _session;
        private readonly IRandom _random;
        //private readonly PlayerSkills _playerSkills;

        public class Factory : IFactory<int, int, Planet>
        {
            [Inject] private readonly DiContainer _container;
            [Inject] private readonly StarMap _starMap;
            [Inject] private readonly IRandom _random;

            public Planet Create(int param1, int param2)
            {
                var args = new object[] { _starMap.GetStarById(param1), param2 };
                return _container.Instantiate<Planet>(args);
            }

            public IEnumerable<Planet> CreatePlanets(int starId)
            {
                var count = _random.RandomInt(starId + 32, 1, 4);
                for (var i = 0; i < count; ++i)
                    yield return Create(starId, i);
            }
        }

        public const int RequiredFuel = 10;
        public const int InfectedPlanetId = 666;
    }
}
