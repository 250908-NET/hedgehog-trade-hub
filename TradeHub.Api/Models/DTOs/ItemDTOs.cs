using FluentValidation;

namespace TradeHub.API.Models.DTOs;

public record ItemDTO(
    long Id,
    string Name,
    string Description,
    string Image,
    decimal Value,
    long OwnerId,
    string Tags,
    string Condition,
    string Availability
);

public record CreateItemDTO(
    string Name,
    string? Description,
    string? Image,
    decimal Value,
    long OwnerId,
    string? Tags,
    string Condition,
    string Availability
);

public record UpdateItemDTO(
    string? Name,
    string? Description,
    string? Image,
    decimal? Value,
    long? OwnerId,
    string? Tags,
    string? Condition,
    string? Availability
);

// --- validators ---

public class CreateItemDTOValidator : AbstractValidator<CreateItemDTO>
{
    public CreateItemDTOValidator()
    {
        RuleFor(i => i.Name).MustBeValidName();
        RuleFor(i => i.Value).MustBeValidValue();
        RuleFor(i => i.OwnerId).MustBeValidOwnerId();
        RuleFor(i => i.Condition).NotEmpty().WithMessage("Condition is required.");
        RuleFor(i => i.Availability).NotEmpty().WithMessage("Availability is required.");
    }
}

public class UpdateItemDTOValidator : AbstractValidator<UpdateItemDTO>
{
    public UpdateItemDTOValidator()
    {
        RuleFor(i => i.Name).MustBeValidName().When(i => i.Name is not null);
        RuleFor(i => i.Value).MustBeValidValue().When(i => i.Value is not null);
        RuleFor(i => i.OwnerId).MustBeValidOwnerId().When(i => i.OwnerId is not null);
        RuleFor(i => i.Condition)
            .NotEmpty()
            .WithMessage("Condition is required.")
            .When(i => i.Condition is not null);
        RuleFor(i => i.Availability)
            .NotEmpty()
            .WithMessage("Availability is required.")
            .When(i => i.Availability is not null);
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
    /// <remarks>
    /// TODO: implement checking if owner exists once UserRepository is implemented
    /// </remarks>
    public static IRuleBuilderOptions<T, long> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
        // .MustAsync(OwnerExists) // async method to check if owner exists
        // .WithMessage("Specified owner does not exist.");
    }

    /// <summary>
    /// Override of <see cref="MustBeValidOwnerId{T}"/> for nullable longs.
    /// </summary>
    /// <remarks>
    /// TODO: implement checking if owner exists once UserRepository is implemented
    /// </remarks>
    public static IRuleBuilderOptions<T, long?> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long?> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
        // .MustAsync(OwnerExists) // async method to check if owner exists
        // .WithMessage("Specified owner does not exist.");
    }

    /// <summary>
    /// Validate Condition is not empty.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidCondition<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Condition is required.");
    }

    /// <summary>
    /// Validate Availability is not empty.
    /// </summary>
    public static IRuleBuilderOptions<T, string?> MustBeValidAvailability<T>(
        this IRuleBuilder<T, string?> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Availability is required.");
    }

    // TODO: figure out if there's a way to handle both TProperty and TProperty? without repeating code
    //       or introducing too much overhead
}
