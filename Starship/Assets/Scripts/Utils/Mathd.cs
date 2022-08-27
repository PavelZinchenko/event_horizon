using System;

namespace Utils
{
    public static class Mathd
    {
        public static double Clamp01(double value)
        {
            if (value < 0.0)
                return 0.0f;
            return value > 1.0 ? 1f : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            if (value < min) value = max;
            else if (value > max) value = max;
            return value;
        }

        public static bool Between(double value, double min, double max)
        {
            return value >= min && value <= max;
        }

        private static int DigitsInDouble(double value)
        {
            if (value == 0 || value == 1) return 1;
            return (int) Math.Floor(Math.Log10(Math.Abs(value)) + 1);
        }

        public static string ToInGameString(this double val)
        {
            if (val < 1e5) return Math.Floor(val).ToString();
            var digs = DigitsInDouble(val) / 3 * 3 - 3;
            val /= Math.Pow(10, digs);

            string ending;
            switch (digs)
            {
                case 0:
                    ending = "";
                    break;
                case 3:
                    ending = "K";
                    break;
                case 6:
                    ending = "M";
                    break;
                case 9:
                    ending = "B";
                    break;
                default:
                    ending = "e" + digs;
                    break;
            }

            return Math.Round(val) + ending;
        }
    }
}
