using System;
using System.Globalization;

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
            return (int)Math.Floor(Math.Log10(Math.Abs(value)) + 1);
        }

        public static string ToInGameString(this double val, BigFormat format)
        {
            switch (format)
            {
                case BigFormat.Truncated:
                    return ToTruncatedInGameString(val);
                case BigFormat.Expanded:
                    return ToExpandedInGameString(val);
                case BigFormat.Decimal:
                    return ToDecimalInGameString(val);
                default:
                    throw new ArgumentOutOfRangeException(nameof(format), format, null);
            }
        }

        public static string ToSignedInGameString(this double val, BigFormat format)
        {
            return (val > 0 ? "+" : "") + val.ToInGameString(format);
        }

        public static string ToTruncatedInGameString(this double val)
        {
            // We don't format very small values for now
            if (Math.Abs(val) < 1) return Math.Truncate(val).ToString(CultureInfo.InvariantCulture);
            // Get amount of digits in steps of 3
            var digs = (DigitsInDouble(val) - 1) / 3 * 3;
            val /= Math.Pow(10, digs);
            return Math.Floor(val).ToString(CultureInfo.InvariantCulture) + GetDigitsSuffix(digs);
        }

        public static string ToExpandedInGameString(this double val)
        {
            if (Math.Abs(val) < 1e5) return Math.Floor(val).ToString(CultureInfo.InvariantCulture);
            var digs = DigitsInDouble(val) / 3 * 3 - 3;
            val /= Math.Pow(10, digs);
            return Math.Floor(val).ToString(CultureInfo.InvariantCulture) + GetDigitsSuffix(digs);
        }

        public static string ToDecimalInGameString(this double val, int significant = 3)
        {
            significant = Math.Max(significant, 1);
            // We don't format small values for now
            if (Math.Abs(val) < 1e5) return Math.Truncate(val).ToString(CultureInfo.InvariantCulture);
            // Get amount of digits in steps of 3
            var digs = (DigitsInDouble(val) - 1) / 3 * 3;

            var significantDigits = DigitsInDouble(val) - significant;
            var result = Math.Floor(val / Math.Pow(10, significantDigits)) * Math.Pow(10, significantDigits - digs);

            return result.ToString(CultureInfo.InvariantCulture) + GetDigitsSuffix(digs);
        }

        private static string GetDigitsSuffix(int digits)
        {
            // Currently hardcoded endings
            switch (digits)
            {
                case 0:
                    return "";
                case 3:
                    return "K";
                case 6:
                    return "M";
                case 9:
                    return "B";
                default:
                    return "e" + digits;
            }
        }

        /// <summary>
        /// Rounds provided value to int, or returns <c>fallback</c> if out of bounds
        /// </summary>
        /// <param name="value">value to round</param>
        /// <param name="fallback">fallback value</param>
        /// <returns>rounded int or fallback value</returns>
        public static int RoundToIntChecked(this double value, int fallback)
        {
            if (value > int.MaxValue || value < int.MinValue) return fallback;
            return (int)Math.Round(value);
        }

        /// <summary>
        /// Rounds provided value to int, respecting int boundaries
        /// </summary>
        /// <param name="value">value to round</param>
        /// <returns>rounded int or fallback value</returns>
        public static int RoundToIntChecked(this double value)
        {
            if (value >= int.MaxValue) return int.MaxValue;
            if (value <= int.MinValue) return int.MinValue;
            return (int)Math.Round(value);
        }
    }

    public enum BigFormat
    {
        /// <summary>
        /// Truncated numerical format, aka 1, 12, 123, 1K, 12K, 123K, 1M
        /// </summary>
        Truncated,

        /// <summary>
        /// Expanded numerical format, aka 1, 12, 123, 1234, 12345, 123K, 1234K, 12345K, 123M
        /// </summary>
        Expanded,

        /// <summary>
        /// Decimal numerical format, aka 1, 12, 123, 1234, 123K, 1.23M, 12.3M, 123M 
        /// </summary>
        Decimal,
    }
}
