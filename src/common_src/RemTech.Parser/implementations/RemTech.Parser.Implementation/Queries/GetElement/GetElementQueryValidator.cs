using FluentValidation;
using RemTech.Parser.Contracts.Contracts.Queries;

namespace RemTech.Parser.Implementation.Queries.GetElement;

public sealed class GetElementQueryValidator : AbstractValidator<GetElementQuery>
{
    public GetElementQueryValidator()
    {
        RuleFor(req => req.Path).NotNull().NotEmpty().WithMessage("Path should be provided");
        RuleFor(req => req.Type).NotNull().NotEmpty().WithMessage("Invalid type");
    }
}
