using System.Collections.Generic;
using System.Linq;
using Constructor.Model;
using Constructor.Ships.Modification;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine.Assertions;
using Utils;

namespace Constructor.Ships
{
    public sealed class ShipModel : IShipModel
    {
        public ShipModel(Ship ship)
        {
            _ship = ship;
            Faction = ship.Faction;

            _layoutModifications = new LayoutModifications(ship);
            _layoutModifications.DataChangedEvent += OnModificationsChanged;

            _modifications = new ObservableCollection<IShipModification>();
            _modifications.DataChangedEvent += OnModificationsChanged;

            OnModificationsChanged();
            DataChanged = false;
        }

        public ShipModel(Ship ship, Faction faction) : this(ship)
        {
            if (faction != null && faction != Faction.Undefined)
                Faction = faction;
        }

        public ShipModel(Ship ship, IEnumerable<IShipModification> modifications, Faction faction) : this(ship)
        {
            if (faction != null && faction != Faction.Undefined)
                Faction = faction;

            if (modifications != null)
                Modifications.Assign(modifications);
        }

        public ItemId<Ship> Id => _ship.Id;
        public ShipCategory Category => _ship.ShipCategory;
        public SizeClass SizeClass => _ship.SizeClass;
        public string OriginalName => _ship.Name;
        public Faction Faction { get; set; }
        public Layout Layout => _stats.Layout;
        public ImmutableCollection<Barrel> Barrels => _ship.Barrels;
        public SpriteId ModelImage => _ship.ModelImage;
        public SpriteId IconImage => _ship.IconImage;
        public float ModelScale => _ship.ModelScale;
        public float IconScale => _ship.IconScale;
        public bool IsBionic => _ship.Regeneration;// || _stats.RegenerationRate > 0;
        public IEnumerable<Device> BuiltinDevices => _stats.BuiltinDevices;

        public bool DataChanged { get; set; }

        public Ship OriginalShip => _ship;

        public ShipBaseStats Stats => _stats;
        public LayoutModifications LayoutModifications => _layoutModifications;
        public IItemCollection<IShipModification> Modifications => _modifications;

        public IShipModel Clone()
        {
            var model = new ShipModel(_ship);
            model.Modifications.Assign(Modifications);
            model.Faction = Faction;
            model.LayoutModifications.Deserialize(LayoutModifications.Serialize().ToArray());
            return model;
        }

        private void OnModificationsChanged()
        {
            _stats = new ShipBaseStats
            {
                BaseWeightMultiplier = new StatMultiplier(_ship.BaseWeightModifier),
                Layout = _layoutModifications.BuildLayout(),
                BuiltinDevices = _ship.BuiltinDevices
            };

            foreach (var modification in _modifications)
                modification.Apply(ref _stats);

            DataChanged = true;
        }

        private readonly Ship _ship;
        private ShipBaseStats _stats;
        private readonly LayoutModifications _layoutModifications;
        private readonly ObservableCollection<IShipModification> _modifications;
    }
}
