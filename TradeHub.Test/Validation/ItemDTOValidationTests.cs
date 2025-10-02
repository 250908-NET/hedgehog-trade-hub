using FluentValidation.TestHelper;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;

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
            Condition: Condition.UsedGood,
            Availability: Availability.Available
        );

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateItemDTOValidator_ShouldBeValid_WhenAllowedEmptyFieldsAreEmpty()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "Not Empty",
            Description: "",
            Image: "",
            Value: 0,
            OwnerId: 1,
            Tags: "",
            Condition: Condition.New,
            Availability: Availability.Available
        );

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public void CreateItemDTOValidator_ShouldHaveValidationErrors_WhenRequiredFieldsAreMissing()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "",
            Description: "",
            Image: "",
            Value: 0,
            OwnerId: 0,
            Tags: "",
            Condition: 0,
            Availability: 0
        );

        // Act
        var result = _validator.TestValidate(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.OwnerId);
    }

    #endregion
}
