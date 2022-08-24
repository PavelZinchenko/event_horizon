using System.ComponentModel;
using GameDatabase.Enums;

namespace Constructor.Modification
{
    //public enum ModificationQuality
    //{
    //    N3 = 0,
    //    N2 = 1,
    //    N1 = 2,
    //    P1 = 3,
    //    P2 = 4,
    //    P3 = 5,
    //}

    public static class QualityExtensions
    {
        public static float PowerMultiplier(this ModificationQuality quality, float n3, float n2, float n1, float p1, float p2, float p3)
        {
            switch (quality)
            {
                case ModificationQuality.N3:
                    return n3;
                case ModificationQuality.N2:
                    return n2;
                case ModificationQuality.N1:
                    return n1;
                case ModificationQuality.P1:
                    return p1;
                case ModificationQuality.P2:
                    return p2;
                case ModificationQuality.P3:
                    return p3;
                default:
                    throw new InvalidEnumArgumentException("quality", (int)quality, typeof(ModificationQuality));
            }
        }
    }
}