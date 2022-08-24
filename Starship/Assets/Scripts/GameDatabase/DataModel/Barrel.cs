using GameDatabase.Serializable;

namespace GameDatabase.DataModel
{
    public partial class Barrel
    {
        public Barrel(BarrelSerializable serializable, Database.Loader loader, int positionInLayout)
            : this(serializable, loader)
        {
            PositionInLayout = positionInLayout;
        }

        public readonly int PositionInLayout;

        public static readonly Barrel Empty = new Barrel();
        private Barrel() { }
    }
  
}
