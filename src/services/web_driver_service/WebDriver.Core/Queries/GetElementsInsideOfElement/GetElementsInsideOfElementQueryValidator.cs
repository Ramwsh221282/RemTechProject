using FluentValidation;

namespace RemTech.WebDriver.Plugin.Queries.GetElementsInsideOfElement;

internal sealed class GetElementsInsideOfElementQueryValidator
    : AbstractValidator<GetElementsInsideOfElementQuery>
{
    public GetElementsInsideOfElementQueryValidator()
    {
        RuleFor(req => req.Query.Path).NotEmpty().NotNull().WithMessage("Path should be provided.");
        RuleFor(req => req.Query.Type).NotEmpty().WithMessage("Type should be provided.");
        RuleFor(req => req.Parent).NotNull().WithMessage("Parent should be provided.");
    }
}
