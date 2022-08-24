using System.Collections.Generic;
using System.Linq;
using Constructor.Ships.Modification;
using GameDatabase.DataModel;
using Utils;

namespace Constructor.Ships
{
    public class CommonShip : BaseShip
    {
        public CommonShip(IShipModel data, IEnumerable<IntegratedComponent> components)
            : base(data)
        {
            _components.Assign(components);
            _components.DataChangedEvent += OnDataChanged;
        }

        public CommonShip(Ship data, IEnumerable<IntegratedComponent> components)
            : base(new ShipModel(data))
        {
            _components.Assign(components);
            _components.DataChangedEvent += OnDataChanged;
        }

        public CommonShip(ShipBuild data, params IShipModification[] modifications)
            : base(new ShipModel(data.Ship, modifications, data.BuildFaction))
        {
            _components.Assign(data.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase));
            _components.DataChangedEvent += OnDataChanged;
        }

        public override string Name
        {
            get { return string.IsNullOrEmpty(_name) ? base.Name : _name; }
            set
            {
                _name = value;
                DataChanged = true;
            }
        }

        public override IItemCollection<IntegratedComponent> Components { get { return _components; } }

        private void OnDataChanged()
        {
            DataChanged = true;
        }

        private readonly ObservableCollection<IntegratedComponent> _components = new ObservableCollection<IntegratedComponent>();
        private string _name;
    }
}
