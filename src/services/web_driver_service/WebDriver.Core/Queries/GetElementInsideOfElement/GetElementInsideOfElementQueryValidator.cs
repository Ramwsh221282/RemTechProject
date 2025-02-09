using FluentValidation;

namespace WebDriver.Core.Queries.GetElementInsideOfElement;

public sealed class GetElementInsideOfElementQueryValidator
    : AbstractValidator<GetElementInsideOfElementQuery>
{
    public GetElementInsideOfElementQueryValidator()
    {
        RuleFor(req => req.ExistingId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("Parent element id is required.");

        RuleFor(req => req.Requested.Path)
            .NotNull()
            .NotEmpty()
            .WithMessage("Requested element path is required");
        RuleFor(req => req.Requested.Type)
            .NotNull()
            .NotEmpty()
            .WithMessage("Requested element type is required");
    }
}
