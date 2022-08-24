using GameDatabase.Enums;
using Services.Localization;

namespace Constructor.Ships
{
    //public enum SizeClass
    //{
    //    Undefined  =-1,
    //    Frigate    = 0,
    //    Destroyer  = 1,
    //    Cruiser    = 2,
    //    Battleship = 3,
    //    Titan      = 4,
    //}

    public static class SizeClassExtensions
    {
        public static float IconSize(this SizeClass sizeClass)
        {
            if (sizeClass == SizeClass.Undefined)
                return 0.5f;

            return 0.4f + 0.15f*(int)sizeClass;
        }

        public static string ToString(this SizeClass sizeClass, ILocalization localization)
        {
            return localization.GetString("$Class" + sizeClass);
        }
    }
}
