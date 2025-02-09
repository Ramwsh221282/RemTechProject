using FluentValidation;

namespace WebDriver.Core.Queries.GetTextFromElement;

public sealed class GetTextFromElementQueryValidator : AbstractValidator<GetTextFromElementQuery>
{
    public GetTextFromElementQueryValidator()
    {
        RuleFor(req => req.ExistingId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("Existing element id is required");
    }
}
