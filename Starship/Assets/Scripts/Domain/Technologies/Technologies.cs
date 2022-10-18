using System.Collections.Generic;
using System.Linq;
using DataModel.Technology;
using Economy.ItemType;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Utils;
using Zenject;

namespace GameServices.Database
{
    public class Technologies : IInitializable, ITechnologies, ITechnologyFactory<ITechnology>
    {
        [Inject] private readonly ItemTypeFactory _factory;
        [Inject] private readonly IDatabase _database;

        [Inject]
        public Technologies(GameDatabaseLoadedSignal databaseLoadedSignal)
        {
            _databaseLoadedSignal = databaseLoadedSignal;
            _databaseLoadedSignal.Event += OnDatabaseLoaded;
        }

        public void Initialize()
        {
            foreach (var item in _database.TechnologyList)
            {
                var tech = item.Create(this);
                _technologies.Add(item.Id.Value, tech);
                foreach (var dependency in item.Dependencies)
                {
                    _dependants.GetOrCreateNew(dependency.Id).Add(tech);
                }
            }
        }

        public ITechnology Get(ItemId<Technology> id)
        {
            if (_technologies.TryGetValue(id.Value, out var technology))
                return technology;

            OptimizedDebug.Log("Technology not found: " + id);
            return null;
        }

        public IEnumerable<ITechnology> All => _technologies.Values;

        public IEnumerable<ITechnology> Dependants(ITechnology root)
        {
            return _dependants.GetOrCreateNew(root.Id);
        }

        public bool TryGetComponentTechnology(Component component, out ITechnology technology)
        {
            var id = component.Id;

            foreach (var item in _technologies)
            {
                if (!(item.Value is ComponentTechnology tech))
                    continue;
                if (tech.Component.Id != id) continue;
                technology = tech;
                return true;
            }

            technology = null;
            return false;
        }

        public bool TryGetShipTechnology(ItemId<Ship> id, out ITechnology technology)
        {
            foreach (var item in _technologies)
            {
                if (!(item.Value is ShipTechnology tech))
                    continue;
                if (tech.Ship.Id != id) continue;
                technology = tech;
                return true;
            }

            technology = null;
            return false;
        }

        public ITechnology Create(Technology_Component content)
        {
            return new ComponentTechnology(this, _factory, content);
        }

        public ITechnology Create(Technology_Ship content)
        {
            return new ShipTechnology(this, _database, _factory, content);
        }

        public ITechnology Create(Technology_Satellite content)
        {
            return new SatelliteTechnology(this, _factory, content);
        }

        private void OnDatabaseLoaded()
        {
            _technologies.Clear();
            _dependants.Clear();
            Initialize();
        }

        private readonly GameDatabaseLoadedSignal _databaseLoadedSignal;
        private readonly Dictionary<int, ITechnology> _technologies = new Dictionary<int, ITechnology>();

        private readonly Dictionary<ItemId<Technology>, HashSet<ITechnology>> _dependants =
            new Dictionary<ItemId<Technology>, HashSet<ITechnology>>();
    }

    public static class TechnologyListExtensions
    {
        public static IEnumerable<ITechnology> Hidden(this IEnumerable<ITechnology> technologies)
        {
            return technologies.Where(item => !item.Hidden);
        }

        public static IEnumerable<ITechnology> OfFaction(this IEnumerable<ITechnology> technologies, Faction faction)
        {
            return technologies.Where(item => faction == Faction.Undefined || item.Faction == faction);
        }

        public static IEnumerable<ITechnology> ForWorkshop(this IEnumerable<ITechnology> technologies, Faction faction)
        {
            return technologies.Where(item => item.Faction == faction || item.Faction == Faction.Neutral);
        }

        public static IEnumerable<ITechnology> Free(this IEnumerable<ITechnology> technologies)
        {
            return technologies.Where(item => item.Price <= 0);
        }
    }
}
