using System;
using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using Utils;

namespace Constructor.Satellites
{
    class EditorModeSatellite : ISatellite
    {
        public EditorModeSatellite(SatelliteBuild build, IDatabase database)
        {
            _database = database;
            _build = build;
        }

        public Satellite Information { get { return _build.Satellite; } }

        public IItemCollection<IntegratedComponent> Components
        {
            get
            {
                if (_components == null)
                {
                    _components = new ObservableCollection<IntegratedComponent>(_build.Components.Select<InstalledComponent,IntegratedComponent>(ComponentExtensions.FromDatabase));
                    _components.DataChangedEvent += OnDataChanged;
                }

                return _components;
            }
        }

        public bool DataChanged { get { return false; } set {} }

        private void OnDataChanged()
        {
            UnityEngine.Debug.Log("EditorModeSatellite.OnDataChanged");

            _build.SetComponents(_components.Select(item => new InstalledComponent(item)));
            _database.SaveSatelliteBuild(_build.Id);
        }

        private ObservableCollection<IntegratedComponent> _components;
        private readonly SatelliteBuild _build;
        private readonly IDatabase _database;
    }
}
