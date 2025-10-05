using System.Globalization;
using System.Text.RegularExpressions;

namespace TradeHub.Api.Utilities;

public static partial class StringExtensions
{
    /// <summary>
    /// Parse a string into a decimal representing a monetary value, and outputs the parsed value to the out parameter,
    /// if successful.
    /// </summary>
    /// <param name="input">The string to parse.</param>
    /// <param name="allowNegative">Whether negative values are allowed. If false and a negative value is found, value will be zero.</param>
    /// <param name="lenientParse">Whether to try to find a valid decimal value, if the string cannot be parsed.</param>
    /// <returns>True if the string could be parsed, false otherwise.</returns>
    public static bool SafeParseMoney(
        this string input,
        out decimal value,
        bool allowNegative = false,
        bool lenientParse = false,
        int precision = 18,
        int scale = 2
    )
    {
        string trimmed = input.Trim();

        CultureInfo usCulture = CultureInfo.GetCultureInfo("en-US"); // Decimal: '.', Group: ','
        CultureInfo deCulture = CultureInfo.GetCultureInfo("de-DE"); // Decimal: ',', Group: '.'

        // simple parse: string is only a valid decimal
        bool success = decimal.TryParse(trimmed, NumberStyles.Number, usCulture, out value);

        // try again with DE number format
        if (!success)
            success = decimal.TryParse(trimmed, NumberStyles.Number, deCulture, out value);

        if (!success && lenientParse)
        {
            // try to return the first valid decimal value in the string, if any exist
            // 1. get the first number-like string
            Match match = FirstNumberlikeString().Match(trimmed);

            // 2. attempt to parse with US number format
            if (match.Success)
                success = decimal.TryParse(match.Value, NumberStyles.Number, usCulture, out value);

            // 3. attempt to parse with DE number format
            if (match.Success && !success)
                success = decimal.TryParse(match.Value, NumberStyles.Number, deCulture, out value);
        }

        // if negative values are not allowed, make sure the value is nonnegative
        if (success && !allowNegative && value < 0)
        {
            success = false;
            value = 0;
        }

        // round to the required precision and scale
        if (success)
            value = Math.Min(
                decimal.Round(value, scale, MidpointRounding.AwayFromZero),
                (precision - scale + 1) * 10m - 1m
            );

        return success;
    }

    [GeneratedRegex(@"[-+]?[\d.,]+")]
    private static partial Regex FirstNumberlikeString();
}
