using FluentValidation;
using TradeHub.Api.Repository.Interfaces;

namespace TradeHub.Api.Models.DTOs;

public record ItemDTO(
    long Id,
    string Name,
    string Description,
    string Image,
    decimal Value,
    long OwnerId,
    string Tags,
    Condition Condition,
    Availability Availability,
    bool IsValueEstimated = false
);

public record CreateItemDTO(
    string Name,
    string Description,
    string Image,
    decimal Value,
    long OwnerId,
    string Tags,
    Condition Condition,
    Availability Availability,
    bool EstimateValue = false
);

public record UpdateItemDTO(
    string? Name,
    string? Description,
    string? Image,
    decimal? Value,
    long? OwnerId,
    string? Tags,
    Condition? Condition,
    Availability? Availability,
    bool EstimateValue = false
);

// --- validators ---

public class CreateItemDTOValidator : AbstractValidator<CreateItemDTO>
{
    private readonly IUserRepository _userRepository; // used for async validation

    public CreateItemDTOValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(i => i.Name).MustBeValidName();
        RuleFor(i => i.Description).NotNull().WithMessage("Description is required.");
        RuleFor(i => i.Image).NotNull().WithMessage("Image is required.");
        RuleFor(i => i.Tags).NotNull().WithMessage("Tags is required.");
        RuleFor(i => i.Value).MustBeValidValue();
        RuleFor(i => i.OwnerId)
            .MustBeValidOwnerId()
            .MustAsync(OwnerExistsAsync) // async method to check if owner exists
            .WithMessage("Specified owner does not exist.");
        RuleFor(i => i.Condition).IsInEnum();
        RuleFor(i => i.Availability).IsInEnum();
    }

    private async Task<bool> OwnerExistsAsync(long ownerId, CancellationToken token)
    {
        return await _userRepository.GetByIdAsync(ownerId) is not null;
    }
}

public class UpdateItemDTOValidator : AbstractValidator<UpdateItemDTO>
{
    private readonly IUserRepository _userRepository;

    public UpdateItemDTOValidator(IUserRepository userRepository)
    {
        _userRepository = userRepository;

        RuleFor(i => i.Name).MustBeValidName().When(i => i.Name is not null);
        RuleFor(i => i.Value).MustBeValidValue().When(i => i.Value is not null);
        RuleFor(i => i.OwnerId)
            .MustBeValidOwnerId()
            .MustAsync(OwnerExistsAsync) // async method to check if owner exists
            .WithMessage("Specified owner does not exist.")
            .When(i => i.OwnerId is not null);
        RuleFor(i => i.Condition).IsInEnum().When(i => i.Condition is not null);
        RuleFor(i => i.Availability).IsInEnum().When(i => i.Availability is not null);
    }

    private async Task<bool> OwnerExistsAsync(long? ownerId, CancellationToken token)
    {
        return ownerId is null || await _userRepository.GetByIdAsync(ownerId.Value) is not null;
    }
}

public static class ItemValidationRules
{
    /// <summary>
    /// Validate name is not empty and not longer than 127 characters.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidName<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        return ruleBuilder
            .NotEmpty()
            .WithMessage("Name is required.")
            .MaximumLength(127)
            .WithMessage("Name cannot exceed 127 characters.");
    }

    /// <summary>
    /// Validate value is nonnegative and fits in a <code>decimal(18, 2)</code>.
    /// </summary>
    public static IRuleBuilderOptions<T, decimal> MustBeValidValue<T>(
        this IRuleBuilder<T, decimal> ruleBuilder
    )
    {
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Value must be nonnegative.");

        return ruleBuilder
            .PrecisionScale(18, 2, true)
            .WithMessage("Value must not have more than 18 digits or more than 2 decimal places.");
    }

    /// <summary>
    /// Override of <see cref="MustBeValidValue{T}"/> for nullable decimals.
    /// </summary>
    public static IRuleBuilderOptions<T, decimal?> MustBeValidValue<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder
    )
    {
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Value must be nonnegative.");

        return ruleBuilder
            .PrecisionScale(18, 2, true)
            .WithMessage("Value must not have more than 18 digits or more than 2 decimal places.");
    }

    /// <summary>
    /// Validate OwnerId is not empty.
    /// </summary>
    public static IRuleBuilderOptions<T, long> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
    }

    /// <summary>
    /// Override of <see cref="MustBeValidOwnerId{T}"/> for nullable longs.
    /// </summary>
    public static IRuleBuilderOptions<T, long?> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long?> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
    }

    // TODO: figure out if there's a way to handle both TProperty and TProperty? without repeating code
    //       or introducing too much overhead
}
