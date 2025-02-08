using FluentValidation;

namespace RemTech.WebDriver.Plugin.Queries.GetElement;

internal sealed class GetElementQueryValidator : AbstractValidator<GetElementQuery>
{
    public GetElementQueryValidator()
    {
        RuleFor(req => req.Path).NotNull().NotEmpty().WithMessage("Path should be provided");
        RuleFor(req => req.Type).NotNull().NotEmpty().WithMessage("Invalid type");
    }
}
