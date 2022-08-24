using System.Collections.Generic;
using Constructor.Model;
using Constructor.Ships.Modification;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Utils;

namespace Constructor.Ships
{
    public interface IShipModel
    {
        ItemId<Ship> Id { get; }
        ShipCategory Category { get; }
        SizeClass SizeClass { get; }
        string OriginalName { get; }
        Faction Faction { get; }
        Layout Layout { get; }
        ImmutableCollection<Barrel> Barrels { get; }
        SpriteId ModelImage { get; }
        SpriteId IconImage { get; }
        float ModelScale { get; }
        float IconScale { get; }
        bool IsBionic { get; }
        IEnumerable<Device> BuiltinDevices { get; }

        ShipBaseStats Stats { get; }

        // TODO: remove
        Ship OriginalShip { get; }

        IItemCollection<IShipModification> Modifications { get; }
        LayoutModifications LayoutModifications { get; }

        bool DataChanged { get; set; }

        IShipModel Clone();
    }
}
