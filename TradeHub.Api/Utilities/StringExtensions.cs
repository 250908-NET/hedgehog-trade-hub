using System.Globalization;
using System.Text.RegularExpressions;

namespace TradeHub.API.Utilities;

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
        this string? input,
        out decimal value,
        bool allowNegative = false,
        bool lenientParse = false,
        int precision = 18,
        int scale = 2
    )
    {
        value = 0;

        if (precision < 0)
            throw new ArgumentOutOfRangeException(
                nameof(precision),
                "Precision must be non-negative."
            );
        if (scale < 0)
            throw new ArgumentOutOfRangeException(nameof(scale), "Scale must be non-negative.");
        if (scale > precision)
            throw new ArgumentOutOfRangeException(
                nameof(scale),
                "Scale must be less than or equal to precision."
            );
        if (input is null)
        {
            return false;
        }

        string candidate = input.Trim();

        CultureInfo firstCulture = GuessCulture(candidate);
        CultureInfo secondCulture =
            firstCulture.Name == "de-DE"
                ? CultureInfo.GetCultureInfo("en-US")
                : CultureInfo.GetCultureInfo("de-DE");

        // simple parse: string is only a valid decimal
        bool success = decimal.TryParse(candidate, NumberStyles.Number, firstCulture, out value);

        // try again with DE number format
        if (!success)
            success = decimal.TryParse(candidate, NumberStyles.Number, secondCulture, out value);

        if (!success && lenientParse)
        {
            // try to return the first valid decimal value in the string, if any exist
            // 1. get the first number-like string
            Match match = FirstNumberlikeString().Match(candidate);

            // 2. try to determine decimal separator

            // 2. attempt to parse with US number format
            if (match.Success)
                success = decimal.TryParse(
                    match.Value,
                    NumberStyles.Number,
                    firstCulture,
                    out value
                );

            // 3. attempt to parse with DE number format
            if (match.Success && !success)
                success = decimal.TryParse(
                    match.Value,
                    NumberStyles.Number,
                    secondCulture,
                    out value
                );
        }

        // apply constraints
        if (success)
        {
            // if negative values are not allowed, make sure the value is nonnegative
            if (!allowNegative && value < 0)
            {
                value = 0;
            }

            // round to specified scale
            value = decimal.Round(value, scale, MidpointRounding.AwayFromZero);

            // truncate to the required precision and scale
            // precision and scale are guaranteed to be good values
            decimal maxMagnitude;

            // If precision is 29 (the maximum for decimal), allow the full range of decimal values,
            // only capping by decimal.MaxValue itself.
            if (precision >= 29)
            {
                maxMagnitude = decimal.MaxValue;
            }
            else
            {
                int integerDigitsCount = precision - scale;

                // calculate maximum integer part
                decimal maxIntegerPart = 1m;
                for (int i = 0; i < integerDigitsCount; i++)
                {
                    maxIntegerPart *= 10m;
                }
                maxIntegerPart -= 1m;

                // calculate the maximum fractional part
                decimal maxFractionalPart = 0m;
                if (scale > 0)
                {
                    decimal unitFraction = 1m;
                    for (int i = 0; i < scale; i++)
                    {
                        unitFraction *= 0.1m;
                    }
                    maxFractionalPart = 1m - unitFraction;
                }
                maxMagnitude = maxIntegerPart + maxFractionalPart;
            }

            if (Math.Abs(value) > maxMagnitude)
                value = Math.Sign(value) * maxMagnitude;
        }

        return success;
    }

    /// <summary>
    /// Tries to guess the culture to use for parsing the string based on some simple heuristics.
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    private static CultureInfo GuessCulture(string input)
    {
        int lastDot = input.LastIndexOf('.');
        int lastComma = input.LastIndexOf(',');

        // if comma is last (and there is a comma), assume DE
        // if only commas, assume DE only if the last comma is 2 characters before the end (0,99, etc.)
        if (lastDot > lastComma && lastComma != -1 || lastDot == -1 && lastComma + 1 < input.Length)
            return CultureInfo.GetCultureInfo("de-DE");

        return CultureInfo.GetCultureInfo("en-US");
    }

    [GeneratedRegex(@"[-+]?[\d.,]+")]
    private static partial Regex FirstNumberlikeString();
}
