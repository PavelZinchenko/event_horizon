using System.ComponentModel;
using GameDatabase.Enums;

namespace Constructor
{
    public enum ComponentQuality
    {
        N3 = 0,
        N2 = 1,
        N1 = 2,
        P0 = 3,
        P1 = 4,
        P2 = 5,
        P3 = 6,
    }

    public static class ComponentQualityExtensions
    {
        public static ComponentQuality Randomize(this ComponentQuality quality, System.Random random)
        {
            var min = quality <= ComponentQuality.N3 ? ComponentQuality.N3 : quality - 1;
            var max = quality >= ComponentQuality.P3 ? ComponentQuality.P3 : quality + 1;
            return (ComponentQuality)random.SquareRange((int) min, (int) max);
        }

        public static int GetLevel(this ComponentQuality quality, int baseLevel)
        {
            switch (quality)
            {
                case ComponentQuality.N3:
                    return 7*baseLevel/10;
                case ComponentQuality.N2:
                    return 8*baseLevel/10;
                case ComponentQuality.N1:
                    return 9*baseLevel/10;
                case ComponentQuality.P0:
                    return baseLevel;
                case ComponentQuality.P1:
                    return 4*baseLevel/3 + 40;
                case ComponentQuality.P2:
                    return 5*baseLevel/3 + 100;
                case ComponentQuality.P3:
                    return 6*baseLevel/3 + 150;
                default:
                    throw new InvalidEnumArgumentException("quality", (int)quality, typeof(ComponentQuality));
            }
        }

        public static ComponentQuality FromLevel(int level, int baseLevel)
        {
            if (level >= GetLevel(ComponentQuality.P3, baseLevel))
                return ComponentQuality.P3;
            if (level >= GetLevel(ComponentQuality.P2, baseLevel))
                return ComponentQuality.P2;
            if (level >= GetLevel(ComponentQuality.P1, baseLevel))
                return ComponentQuality.P1;
            if (level <= GetLevel(ComponentQuality.N3, baseLevel))
                return ComponentQuality.N3;
            if (level <= GetLevel(ComponentQuality.N2, baseLevel))
                return ComponentQuality.N2;
            if (level <= GetLevel(ComponentQuality.N1, baseLevel))
                return ComponentQuality.N1;

            return ComponentQuality.P0;
        }

        public static ModificationQuality ToModificationQuality(this ComponentQuality quality)
        {
            switch (quality)
            {
                case ComponentQuality.N3:
                    return ModificationQuality.N3;
                case ComponentQuality.N2:
                    return ModificationQuality.N2;
                case ComponentQuality.N1:
                    return ModificationQuality.N1;
                case ComponentQuality.P1:
                    return ModificationQuality.P1;
                case ComponentQuality.P2:
                    return ModificationQuality.P2;
                case ComponentQuality.P3:
                    return ModificationQuality.P3;
                default:
                    throw new InvalidEnumArgumentException("quality", (int)quality, typeof(ComponentQuality));
            }
        }
    }
}
