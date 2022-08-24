using UnityEngine;

namespace Maths
{
    public static class Format
    {
        public static string SignedPercent(float value)
        {
            return value.ToString("+#%;-#%;+0%");
        }

        public static string SignedFloat(float value)
        {
            return value.ToString("+0.##;-0.##");
        }
    }
}
