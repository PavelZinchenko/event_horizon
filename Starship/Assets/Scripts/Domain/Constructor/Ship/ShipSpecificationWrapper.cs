using System.Collections.Generic;
using Constructor.Model;

namespace Constructor
{
    public class ShipSpecificationWrapper : IShipSpecification
    {
        protected ShipSpecificationWrapper() { }
        protected virtual IShipSpecification Specs { get; set; }
        public ShipType Type => Specs.Type;
        public IShipStats Stats => Specs.Stats;
        public IEnumerable<IWeaponPlatformData> Platforms => Specs.Platforms;
        public IEnumerable<IDeviceData> Devices => Specs.Devices;
        public IEnumerable<IDroneBayData> DroneBays => Specs.DroneBays;
        public IShipSpecification CopyWithStats(IShipStats stats) => Specs.CopyWithStats(stats);

        public static ShipSpecificationWrapper Wrap(IShipSpecification specs)
        {
            var wrapper = new ShipSpecificationWrapper();
            if (specs is ShipSpecificationWrapper wrappedSpecs) wrapper.Specs = wrappedSpecs.Specs;
            else wrapper.Specs = specs;

            return wrapper;
        }
    }

    public delegate IShipSpecification SpecsProvider();

    public class LazyShipSpecification : ShipSpecificationWrapper
    {
        private SpecsProvider _provider;
        private IShipSpecification _cache;

        public LazyShipSpecification(SpecsProvider provider)
        {
            _provider = provider;
        }

        protected override IShipSpecification Specs
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
