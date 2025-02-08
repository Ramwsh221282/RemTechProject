using FluentValidation;

namespace RemTech.WebDriver.Plugin.Queries.GetElementInsideOfElement;

internal sealed class GetElementInsideOfElementQueryValidator
    : AbstractValidator<GetElementInsideOfElementQuery>
{
    public GetElementInsideOfElementQueryValidator()
    {
        RuleFor(req => req.Element).NotNull().WithMessage("Parent element is required.");
        RuleFor(req => req.Query.Path).NotNull().NotEmpty().WithMessage("Query path is required.");
        RuleFor(req => req.Query.Type).NotNull().WithMessage("Query type is required.");
    }
}
