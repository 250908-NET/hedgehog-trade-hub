using FluentValidation.TestHelper;
using TradeHub.API.Models.DTOs;

namespace TradeHub.Test.Validation;

public class ItemDTOValidationTests
{
    #region CreateItemDTOValidator tests
    private readonly CreateItemDTOValidator _validator = new();

    [Fact]
    public void CreateItemDTOValidator_ShouldBeValid_WhenAllFieldsAreValid()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "Test Item",
            Description: "This is a test item.",
            Image: "https://example.com/test.jpg",
            Value: 9.99m,
            OwnerId: 1,
            Tags: "tag1,tag2,tag3",
            Condition: "Good",
            Availability: "Available"
        );

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    #endregion
}
