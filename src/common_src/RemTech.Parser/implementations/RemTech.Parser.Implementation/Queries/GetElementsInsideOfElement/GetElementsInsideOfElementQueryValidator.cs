using FluentValidation;
using RemTech.Parser.Contracts.Contracts.Queries;

namespace RemTech.Parser.Implementation.Queries.GetElementsInsideOfElement;

public sealed class GetElementsInsideOfElementQueryValidator
    : AbstractValidator<GetElementsInsideOfElementQuery>
{
    public GetElementsInsideOfElementQueryValidator()
    {
        RuleFor(req => req.Query.Path).NotEmpty().NotNull().WithMessage("Path should be provided.");
        RuleFor(req => req.Query.Type).NotEmpty().WithMessage("Type should be provided.");
        RuleFor(req => req.Parent).NotNull().WithMessage("Parent should be provided.");
    }
}
