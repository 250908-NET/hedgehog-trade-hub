using TradeHub.Api.Utilities;

namespace TradeHub.Test.Utilities;

public class StringExtensionsTests
{
    #region Basic Parsing Tests

    [Theory]
    [InlineData("123.45", 123.45)] // US culture
    [InlineData("1000", 1000.00)] // Integer
    [InlineData("0.99", 0.99)] // Decimal less than 1
    [InlineData("1,234.56", 1234.56)] // US culture with group separator
    [InlineData("  50.00  ", 50.00)] // With whitespace
    public void SafeParseMoney_ShouldParseValidUsCultureStrings(string input, decimal expectedValue)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("123,45", 123.45)] // DE culture
    [InlineData("1.000,00", 1000.00)] // DE culture with group separator
    [InlineData("0,99", 0.99)] // DE culture decimal less than 1
    public void SafeParseMoney_ShouldParseValidDeCultureStrings(string input, decimal expectedValue)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    #endregion

    #region Lenient Parsing Tests

    [Theory]
    [InlineData("Price: $123.45 USD", 123.45)]
    [InlineData("Item value is 99,99 EUR", 99.99)]
    [InlineData("Approximately 1,000.50 units", 1000.50)]
    [InlineData("Only 5.00 left!", 5.00)]
    [InlineData("Value is .75", 0.75)]
    [InlineData("Value is ,75", 0.75)] // Should handle comma as decimal separator
    public void SafeParseMoney_ShouldLenientlyParseStringsWithExtraText(
        string input,
        decimal expectedValue
    )
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, lenientParse: true);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("The price is 123.45 and another is 67.89", 123.45)] // Should get the first
    [InlineData("First 100, then 200", 100.00)]
    public void SafeParseMoney_ShouldLenientlyParseFirstNumber(string input, decimal expectedValue)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, lenientParse: true);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    #endregion

    #region Negative Value Tests

    [Theory]
    [InlineData("-10.50", -10.50)]
    [InlineData("-1.234,56", -1234.56)] // DE culture negative
    public void SafeParseMoney_ShouldParseNegativeValues_WhenAllowed(
        string input,
        decimal expectedValue
    )
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, allowNegative: true);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    [Theory]
    [InlineData("-10.50")]
    [InlineData("-1.234,56")]
    [InlineData("Value is -50", true)] // Lenient parse
    public void SafeParseMoney_ShouldReturnZeroAndTrue_WhenNegativeValuesAreDisallowed(
        string input,
        bool lenientParse = false
    )
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(
            out actualValue,
            allowNegative: false,
            lenientParse: lenientParse
        );

        // Assert
        Assert.True(success);
        Assert.Equal(0m, actualValue);
    }

    #endregion

    #region Precision and Scale Tests

    [Theory]
    [InlineData("123.456", 123.46)] // Rounds up
    [InlineData("123.454", 123.45)] // Rounds down
    [InlineData("123.455", 123.46)] // Midpoint rounding (AwayFromZero)
    [InlineData("123", 123.00)] // Adds scale
    public void SafeParseMoney_ShouldRoundToCorrectScale(string input, decimal expectedValue)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, scale: 2);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldHandleDifferentScale()
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = "123.4567".SafeParseMoney(out actualValue, scale: 4);

        // Assert
        Assert.True(success);
        Assert.Equal(123.4567m, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldCapValue_WhenExceedsSpecifiedPrecision()
    {
        // Arrange
        decimal actualValue;
        // For precision: 3, scale: 2, the max value is 9.99
        string inputExceedingPrecision = "169.00";
        decimal expectedCappedValue = 9.99m;

        // Act
        bool success = inputExceedingPrecision.SafeParseMoney(
            out actualValue,
            precision: 3,
            scale: 2
        );

        // Assert
        Assert.True(success);
        Assert.Equal(expectedCappedValue, actualValue);

        // Test a negative value exceeding precision
        string negativeInputExceedingPrecision = "-169.00";
        decimal expectedNegativeCappedValue = -9.99m;
        success = negativeInputExceedingPrecision.SafeParseMoney(
            out actualValue,
            precision: 3,
            scale: 2,
            allowNegative: true
        );
        Assert.True(success);
        Assert.Equal(expectedNegativeCappedValue, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldNotCapValue_WhenWithinSpecifiedPrecision()
    {
        // Arrange
        decimal actualValue;
        // For precision: 3, scale: 2, the max value is 9.99
        string inputWithinPrecision = "5.45";
        decimal expectedValue = 5.45m;

        // Act
        bool success = inputWithinPrecision.SafeParseMoney(out actualValue, precision: 3, scale: 2);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    #endregion

    #region Edge Case Tests

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void SafeParseMoney_ShouldReturnFalseAndZero_ForNullOrWhitespace(string? input)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue);

        // Assert
        Assert.False(success);
        Assert.Equal(0m, actualValue);
    }

    [Theory]
    [InlineData("abc")]
    [InlineData("no numbers here")]
    [InlineData("$,-")]
    public void SafeParseMoney_ShouldReturnFalseAndZero_ForStringsWithoutNumbers(string input)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, lenientParse: true); // Test with lenient parse

        // Assert
        Assert.False(success);
        Assert.Equal(0m, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldHandleLargeNumbersWithinDecimalLimits()
    {
        // Arrange
        decimal actualValue;
        string largeNumber = "79228162514264337593543950000.00"; // Max decimal value
        decimal expectedValue = 79228162514264337593543950000.00m;

        // Act
        bool success = largeNumber.SafeParseMoney(out actualValue, precision: 29, scale: 2); // Max precision

        // Assert
        Assert.True(success);
        Assert.Equal(expectedValue, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldHandleOverflowIfInputExceedsDecimalLimits()
    {
        // Arrange
        decimal actualValue;
        string overflowNumber = "792281625142643375935439500000.00"; // Exceeds max decimal value

        // Act
        bool success = overflowNumber.SafeParseMoney(out actualValue, precision: 29, scale: 2);

        // Assert
        // decimal.TryParse will return false on overflow
        Assert.False(success);
        Assert.Equal(0m, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldHandleZeroScale()
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = "123.456".SafeParseMoney(out actualValue, scale: 0);

        // Assert
        Assert.True(success);
        Assert.Equal(123m, actualValue); // Should round to nearest integer
    }

    [Theory]
    [InlineData("Price: $123.45 USD")]
    public void SafeParseMoney_ShouldReturnFalse_WhenNotLenientAndContainsExtraText(string input)
    {
        // Arrange
        decimal actualValue;

        // Act
        bool success = input.SafeParseMoney(out actualValue, lenientParse: false);

        // Assert
        Assert.False(success);
        Assert.Equal(0m, actualValue);
    }

    [Fact]
    public void SafeParseMoney_ShouldCapValue_WhenPrecisionEqualsScale()
    {
        // Arrange
        decimal actualValue;
        // For precision: 2, scale: 2, the max value is 0.99 (integerDigitsCount = 0)
        string inputExceedingPrecision = "1.23";
        decimal expectedCappedValue = 0.99m;

        // Act
        bool success = inputExceedingPrecision.SafeParseMoney(
            out actualValue,
            precision: 2,
            scale: 2
        );

        // Assert
        Assert.True(success);
        Assert.Equal(expectedCappedValue, actualValue);
    }

    #endregion

    #region Invalid Parameter Tests

    [Theory]
    [InlineData(-1, 2, "precision")]
    [InlineData(5, -1, "scale")]
    public void SafeParseMoney_ShouldThrowArgumentOutOfRangeException_ForNegativePrecisionOrScale(
        int precision,
        int scale,
        string paramName
    )
    {
        // Arrange
        decimal actualValue;
        string input = "10.00";

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            input.SafeParseMoney(out actualValue, precision: precision, scale: scale)
        );
        Assert.Equal(paramName, ex.ParamName);
    }

    [Fact]
    public void SafeParseMoney_ShouldThrowArgumentOutOfRangeException_WhenScaleExceedsPrecision()
    {
        // Arrange
        decimal actualValue;
        string input = "10.00";

        // Act & Assert
        var ex = Assert.Throws<ArgumentOutOfRangeException>(() =>
            input.SafeParseMoney(out actualValue, precision: 2, scale: 3)
        );
        Assert.Equal("scale", ex.ParamName);
    }

    #endregion
}
