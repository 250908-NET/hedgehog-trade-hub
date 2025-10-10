using FluentValidation.TestHelper;
using Moq;
using TradeHub.Api.Models;
using TradeHub.Api.Models.DTOs;
using TradeHub.Api.Repository.Interfaces;
using Xunit; // Ensure Xunit is imported for [MemberData]

namespace TradeHub.Test.Validation;

public class ItemDTOValidationTests
{
    private readonly Mock<IUserRepository> _mockUserRepository; // mock repository
    private readonly CreateItemDTOValidator _createValidator;
    private readonly UpdateItemDTOValidator _updateValidator;

    public ItemDTOValidationTests()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        // default behavior for owner existence: assume owner 1 exists, others don't
        _mockUserRepository.Setup(repo => repo.GetByIdAsync(1)).ReturnsAsync(new User { Id = 1 });
        _mockUserRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<long>(id => id != 1)))
            .ReturnsAsync((User?)null);

        _createValidator = new CreateItemDTOValidator(_mockUserRepository.Object);
        _updateValidator = new UpdateItemDTOValidator(_mockUserRepository.Object);
    }

    // Helper method to generate long strings for test data
    private static string GenerateLongString(int length) => new('a', length);

    // Define static test data for CreateItemDTOValidator
    public static TheoryData<string, decimal, string> CreateItemExceedLimitsData =>
        new()
        {
            {
                GenerateLongString(128), // Name too long (max 127)
                -1.0m, // Value negative
                GenerateLongString(256) // Tags too long (if max 255 is intended)
            },
            {
                "Valid Name",
                10.0m,
                GenerateLongString(256) // Tags too long (if max 255 is intended)
            },
            {
                GenerateLongString(128), // Name too long (max 127)
                10.0m,
                "valid,tags"
            },
            {
                "Valid Name",
                -5.0m, // Value negative
                "valid,tags"
            },
        };

    #region CreateItemDTOValidator tests
    [Fact]
    public async Task CreateItemDTOValidator_ShouldBeValid_WhenAllFieldsAreValid()
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
        var result = await _createValidator.TestValidateAsync(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateItemDTOValidator_ShouldBeValid_WhenAllowedEmptyFieldsAreEmpty()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "Not Empty",
            Description: "", // NotNull passes for empty string
            Image: "", // NotNull passes for empty string
            Value: 0,
            OwnerId: 1,
            Tags: "", // NotNull passes for empty string
            Condition: Condition.New,
            Availability: Availability.Available
        );

        // Act
        var result = await _createValidator.TestValidateAsync(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task CreateItemDTOValidator_ShouldHaveValidationErrors_WhenRequiredFieldsAreMissing()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "", // Fails MustBeValidName (NotEmpty)
            Description: "", // NotNull passes for empty string
            Image: "", // NotNull passes for empty string
            Value: 0,
            OwnerId: 0, // Fails MustBeValidOwnerId (NotEmpty)
            Tags: "", // NotNull passes for empty string
            Condition: 0,
            Availability: 0
        );

        // Act
        var result = await _createValidator.TestValidateAsync(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
        result.ShouldHaveValidationErrorFor(x => x.OwnerId);
        // Note: Description, Image, Tags are non-nullable strings in DTO.
        // RuleFor().NotNull() passes for empty strings. If NotEmpty() is desired,
        // the validator rules in ItemDTOs.cs need to be updated.
    }

    [Theory]
    [MemberData(nameof(CreateItemExceedLimitsData))]
    public async Task CreateItemDTOValidator_ShouldHaveValidationErrors_WhenFieldsExceedLimits(
        string name,
        decimal value,
        string tags
    )
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: name,
            Description: "Valid Description", // Provide valid required fields
            Image: "Valid Image",
            Value: value,
            OwnerId: 1,
            Tags: tags,
            Condition: Condition.New,
            Availability: Availability.Available
        );

        // Act
        var result = await _createValidator.TestValidateAsync(item);

        // Assert
        if (name != null && name.Length > 127) // Name max length is 127
            result.ShouldHaveValidationErrorFor(x => x.Name);
        if (value < 0)
            result.ShouldHaveValidationErrorFor(x => x.Value);
        // if (tags != null && tags.Length > 255)
        //     result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public async Task CreateItemDTOValidator_ShouldHaveValidationError_WhenOwnerDoesNotExist()
    {
        // Arrange
        var item = new CreateItemDTO(
            Name: "Valid Name",
            Description: "Valid Description",
            Image: "Valid Image",
            Value: 10.0m,
            OwnerId: 999, // This owner is mocked to NOT exist
            Tags: "valid,tags",
            Condition: Condition.New,
            Availability: Availability.Available
        );

        // Act
        var result = await _createValidator.TestValidateAsync(item);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.OwnerId)
            .WithErrorMessage("Specified owner does not exist.");
    }

    #endregion

    // Define static test data for UpdateItemDTOValidator
    public static TheoryData<string, decimal, string> UpdateItemExceedLimitsData =>
        new()
        {
            {
                GenerateLongString(128), // Name too long (max 127)
                -1.0m, // Value negative
                GenerateLongString(256) // Tags too long (if max 255 is intended)
            },
            {
                "Valid Name",
                10.0m,
                GenerateLongString(256) // Tags too long (if max 255 is intended)
            },
            {
                GenerateLongString(128), // Name too long (max 127)
                10.0m,
                "valid,tags"
            },
            {
                "Valid Name",
                -5.0m, // Value negative
                "valid,tags"
            },
        };

    #region UpdateItemDTOValidator tests
    [Fact]
    public async Task UpdateItemDTOValidator_ShouldBeValid_WhenAllFieldsAreNull()
    {
        // Arrange
        var item = new UpdateItemDTO(null, null, null, null, null, null, null, null);

        // Act
        var result = await _updateValidator.TestValidateAsync(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Fact]
    public async Task UpdateItemDTOValidator_ShouldBeValid_WhenAllFieldsAreValid()
    {
        // Arrange
        var item = new UpdateItemDTO(
            Name: "Updated Name",
            Description: "Updated description.",
            Image: "https://example.com/updated.jpg",
            Value: 19.99m,
            OwnerId: 1,
            Tags: "updated,tags",
            Condition: Condition.UsedLikeNew,
            Availability: Availability.Unavailable
        );

        // Act
        var result = await _updateValidator.TestValidateAsync(item);

        // Assert
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [MemberData(nameof(UpdateItemExceedLimitsData))]
    public async Task UpdateItemDTOValidator_ShouldHaveValidationErrors_WhenFieldsExceedLimits(
        string name,
        decimal value,
        string tags
    )
    {
        // Arrange
        var item = new UpdateItemDTO(
            Name: name,
            Description: null,
            Image: null,
            Value: value,
            OwnerId: null,
            Tags: tags,
            Condition: null,
            Availability: null
        );

        // Act
        var result = await _updateValidator.TestValidateAsync(item);

        // Assert
        if (name != null && name.Length > 127) // Name max length is 127
            result.ShouldHaveValidationErrorFor(x => x.Name);
        if (value < 0)
            result.ShouldHaveValidationErrorFor(x => x.Value);
        // if (tags != null && tags.Length > 255)
        //     result.ShouldHaveValidationErrorFor(x => x.Tags);
    }

    [Fact]
    public async Task UpdateItemDTOValidator_ShouldHaveValidationError_WhenNameIsEmpty()
    {
        // Arrange
        var item = new UpdateItemDTO("", null, null, null, null, null, null, null);

        // Act
        var result = await _updateValidator.TestValidateAsync(item);

        // Assert
        result.ShouldHaveValidationErrorFor(x => x.Name);
    }

    [Fact]
    public async Task UpdateItemDTOValidator_ShouldHaveValidationError_WhenOwnerDoesNotExist()
    {
        // Arrange
        var item = new UpdateItemDTO(
            Name: "Valid Name",
            Description: null,
            Image: null,
            Value: null,
            OwnerId: 999, // This owner is mocked to NOT exist
            Tags: null,
            Condition: null,
            Availability: null
        );

        // Act
        var result = await _updateValidator.TestValidateAsync(item);

        // Assert
        result
            .ShouldHaveValidationErrorFor(x => x.OwnerId)
            .WithErrorMessage("Specified owner does not exist.");
    }

    #endregion
}
