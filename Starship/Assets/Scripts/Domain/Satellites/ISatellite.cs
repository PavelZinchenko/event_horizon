using GameDatabase.DataModel;
using Utils;

namespace Constructor.Satellites
{
    public interface ISatellite
    {
        Satellite Information { get; }
        IItemCollection<IntegratedComponent> Components { get; }

        bool DataChanged { get; set; }
    }
}
