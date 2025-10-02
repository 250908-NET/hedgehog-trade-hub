using System.ComponentModel;
using System.Runtime.CompilerServices;
using FluentValidation;

namespace TradeHub.API.Models.DTOs;

public record ItemDTO(
    long Id,
    string Name,
    string Description,
    string Image,
    decimal Value,
    long OwnerId,
    string Tags
);

public record CreateItemDTO(
    string Name,
    string? Description,
    string? Image,
    decimal Value,
    long OwnerId,
    string? Tags
);

public record UpdateItemDTO(
    string? Name,
    string? Description,
    string? Image,
    decimal? Value,
    long? OwnerId,
    string? Tags
);

// --- validators ---

public class CreateItemDTOValidator : AbstractValidator<CreateItemDTO>
{
    public CreateItemDTOValidator()
    {
        RuleFor(i => i.Name).MustBeValidName();
        RuleFor(i => i.Value).MustBeValidValue();
        RuleFor(i => i.OwnerId).MustBeValidOwnerId();
    }
}

public class UpdateItemDTOValidator : AbstractValidator<UpdateItemDTO>
{
    public UpdateItemDTOValidator()
    {
        RuleFor(i => i.Name).MustBeValidName().When(i => i.Name is not null);
        RuleFor(i => i.Value).MustBeValidValue().When(i => i.Value is not null);
        RuleFor(i => i.OwnerId).MustBeValidOwnerId().When(i => i.OwnerId is not null);
    }
}

public static class ItemValidationRules
{
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

    public static IRuleBuilderOptions<T, decimal> MustBeValidValue<T>(
        this IRuleBuilder<T, decimal> ruleBuilder
    )
    {
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Value must be nonnegative.");

        return ruleBuilder
            .PrecisionScale(18, 2, true)
            .WithMessage("Value must not have more than 18 digits or more than 2 decimal places.");
    }

    public static IRuleBuilderOptions<T, decimal?> MustBeValidValue<T>(
        this IRuleBuilder<T, decimal?> ruleBuilder
    )
    {
        ruleBuilder.GreaterThanOrEqualTo(0).WithMessage("Value must be nonnegative.");

        return ruleBuilder
            .PrecisionScale(18, 2, true)
            .WithMessage("Value must not have more than 18 digits or more than 2 decimal places.");
    }

    // TODO: implement checking if owner exists once UserRepository is implemented
    public static IRuleBuilderOptions<T, long> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
        // .MustAsync(OwnerExists) // async method to check if owner exists
        // .WithMessage("Specified owner does not exist.");
    }

    // TODO: implement checking if owner exists once UserRepository is implemented
    public static IRuleBuilderOptions<T, long?> MustBeValidOwnerId<T>(
        this IRuleBuilder<T, long?> ruleBuilder
    )
    {
        return ruleBuilder.NotEmpty().WithMessage("Owner is required.");
        // .MustAsync(OwnerExists) // async method to check if owner exists
        // .WithMessage("Specified owner does not exist.");
    }

    // TODO: figure out if there's a way to handle both TProperty and TProperty? without repeating code
    //       or introducing too much overhead
}
