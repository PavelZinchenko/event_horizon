using Constructor.Satellites;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Maths;
using Utils;

namespace Constructor.Ships
{
    public class ShipWrapper : IShip
    {
        protected virtual IShip Ship { get; set; }

        public static ShipWrapper Wrap(IShip ship)
        {
            var wrapped = new ShipWrapper();
            if (ship is ShipWrapper wrapper) wrapped.Ship = wrapper.Ship;
            wrapped.Ship = ship;
            return wrapped;
        }
        
        public ItemId<Ship> Id => Ship.Id;
        public string Name
        {
            get => Ship.Name;
            set => Ship.Name = value;
        }

        public ShipColorScheme ColorScheme => Ship.ColorScheme;
        public IShipModel Model => Ship.Model;
        public IItemCollection<IntegratedComponent> Components => Ship.Components;
        public ISatellite FirstSatellite
        {
            get => Ship.FirstSatellite;
            set => Ship.FirstSatellite = value;
        }
        public ISatellite SecondSatellite 
        {
            get => Ship.SecondSatellite;
            set => Ship.SecondSatellite = value;
        }

        public DifficultyClass ExtraThreatLevel => Ship.ExtraThreatLevel;

        public Experience Experience
        {
            get => Ship.Experience;
            set => Ship.Experience = value;
        }

        public ShipBuilder CreateBuilder() => Ship.CreateBuilder();

        public bool DataChanged
        {
            get => Ship.DataChanged;
            set => Ship.DataChanged = value;
        }

        public int RemoveInvalidComponents(IGameItemCollection<ComponentInfo> inventory) =>
            Ship.RemoveInvalidComponents(inventory);
    }

    public delegate IShip ShipProvider();
    public class LazyShip : ShipWrapper
    {
        private ShipProvider _provider;
        private IShip _cache;
        public LazyShip(ShipProvider provider)
        {
            _provider = provider;
        }

        protected override IShip Ship
        {
            get
            {
                if (_cache != null) return _cache;
                _cache = _provider();
                _provider = null;

                return _cache;
            }
            set => _cache = value;
        }
    }
}
