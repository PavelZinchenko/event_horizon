using GameDatabase.Model;
using GameDatabase.Serializable;
using GameDatabase.Utils;

namespace GameDatabase.DataModel
{
    public partial class Ship
    {
        partial void OnDataDeserialized(ShipSerializable serializable, Database.Loader loader)
        {
            if (_engineSize > 0)
            {
                var engine = new EngineSerializable { Position = _enginePosition, Size = _engineSize };
                Engines = Engine.Create(engine, loader) + Engines;
            }

            Barrels = new ImmutableCollection<Barrel>(BarrelConverter.Convert(Layout, serializable.Barrels));
        }
    }
}
