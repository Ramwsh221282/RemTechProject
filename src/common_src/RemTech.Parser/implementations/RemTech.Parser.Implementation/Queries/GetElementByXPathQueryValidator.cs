using FluentValidation;
using RemTech.Parser.Contracts.Contracts.Queries;

namespace RemTech.Parser.Implementation.Queries;

public sealed class GetElementByXPathQueryValidator : AbstractValidator<GetElementQuery>
{
    public GetElementByXPathQueryValidator()
    {
        RuleFor(req => req.Path).NotNull().NotEmpty().WithMessage("Path should be provided");
        RuleFor(req => req.Type).NotNull().NotEmpty().WithMessage("Invalid type");
    }
}
