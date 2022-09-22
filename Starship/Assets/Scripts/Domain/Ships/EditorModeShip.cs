using System.Linq;
using GameDatabase;
using GameDatabase.DataModel;
using Utils;

namespace Constructor.Ships
{
    public class EditorModeShip : BaseShip
    {
        public EditorModeShip(ShipBuild build, IDatabase database)
            : base(new ShipModel(build.Ship, Faction.Undefined))
        {
            _database = database;
            _shipBuild = build;
        }

        public override IItemCollection<IntegratedComponent> Components
        {
            get
            {
                if (_components == null)
                {
                    _components = new ObservableCollection<IntegratedComponent>(_shipBuild.Components.Select<InstalledComponent,IntegratedComponent>(item =>
                    {
                        var component = ComponentExtensions.FromDatabase(item);
                        component.Locked = false;
                        return component;
                    }));

                    _components.DataChangedEvent += SaveComponents;
                }

                return _components;
            }
        }

        public override string Name { get { return _shipBuild.Id.ToString(); } set {} }

        private void SaveComponents()
        {
            OptimizedDebug.Log("EditorModeShip.SaveComponents");
            _shipBuild.SetComponents(_components.Select(item => new InstalledComponent(item)));
            _database.SaveShipBuild(_shipBuild.Id);
        }

        private ObservableCollection<IntegratedComponent> _components;
        private readonly ShipBuild _shipBuild;
        private readonly IDatabase _database;
    }
}
