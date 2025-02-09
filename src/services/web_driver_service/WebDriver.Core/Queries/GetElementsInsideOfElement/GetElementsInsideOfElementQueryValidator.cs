using FluentValidation;

namespace WebDriver.Core.Queries.GetElementsInsideOfElement;

public sealed class GetElementsInsideOfElementQueryValidator
    : AbstractValidator<GetElementsInsideOfElementQuery>
{
    public GetElementsInsideOfElementQueryValidator()
    {
        RuleFor(req => req.ExistingId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("Parent Id is required");

        RuleFor(req => req.Requested.Path)
            .NotNull()
            .NotEmpty()
            .WithMessage("Requested Path is required");
        RuleFor(req => req.Requested.Type)
            .NotNull()
            .NotEmpty()
            .WithMessage("Requested Type is required");
    }
}
