using System;
using System.Globalization;

namespace RIEVES.GGJ2026.Core.Utilities
{
    public static class StringUtilities
    {
        // 4955 -> 5k WRONG
        public static string ToAbbreviatedString(
            this float value,
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000f) / 10f).ToString(billionFormat);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000f) / 10f).ToString(millionFormat);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100f) / 10f).ToString(thousandFormat);
            }

            return value.ToString(fallbackFormat);
        }

        public static string ToAbbreviatedString(
            this double value,
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000d) / 10d).ToString(billionFormat);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000d) / 10d).ToString(millionFormat);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100d) / 10d).ToString(thousandFormat);
            }

            return value.ToString(fallbackFormat);
        }

        public static string ToAbbreviatedString(
            this long value,
            string billionFormat = "0.#'B'",
            string millionFormat = "0.#'M'",
            string thousandFormat = "0.#'K'",
            string fallbackFormat = "0.##"
        )
        {
            if (value >= 1_000_000_000)
            {
                return (Math.Truncate(value / 100_000_000d) / 10d).ToString(billionFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 1_000_000)
            {
                return (Math.Truncate(value / 100_000d) / 10d).ToString(millionFormat, CultureInfo.InvariantCulture);
            }

            if (value >= 1_000)
            {
                return (Math.Truncate(value / 100d) / 10d).ToString(thousandFormat, CultureInfo.InvariantCulture);
            }

            return value.ToString(fallbackFormat, CultureInfo.InvariantCulture);
        }
    }
}
