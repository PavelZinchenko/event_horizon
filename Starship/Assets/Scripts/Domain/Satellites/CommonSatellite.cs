using System.Collections.Generic;
using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Model;
using Utils;
using Helpers = GameModel.Serialization.Helpers;

namespace Constructor.Satellites
{
    public class CommonSatellite : ISatellite
    {
        public CommonSatellite(Satellite satellite, IEnumerable<IntegratedComponent> components)
        {
            Information = satellite;
            _components.Assign(components);
            _components.DataChangedEvent += OnDataChanged;
        }

        public CommonSatellite(SatelliteBuild build)
        {
            Information = build.Satellite;
            _components.Assign(build.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase));
            _components.DataChangedEvent += OnDataChanged;
        }

        public Satellite Information { get; private set; }

        public IItemCollection<IntegratedComponent> Components { get { return _components; } }

        public bool DataChanged { get; set; }

        private void OnDataChanged()
        {
            DataChanged = true;
        }

        private ObservableCollection<IntegratedComponent> _components = new ObservableCollection<IntegratedComponent>();
    }
}
