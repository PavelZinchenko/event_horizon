using GameDatabase.Model;
using GameDatabase.Serializable;
using GameDatabase.Utils;

namespace GameDatabase.DataModel
{
    public partial class Satellite
    {
        partial void OnDataDeserialized(SatelliteSerializable serializable, Database.Loader loader)
        {
            Barrels = new ImmutableCollection<Barrel>(BarrelConverter.Convert(Layout, serializable.Barrels));
        }
    }
}
