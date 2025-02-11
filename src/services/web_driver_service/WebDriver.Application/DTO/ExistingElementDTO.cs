using FluentValidation;

namespace WebDriver.Application.DTO;

public sealed record ExistingElementDTO(Guid ExistingId);

internal sealed class ExistingElementDTOValidator : AbstractValidator<ExistingElementDTO>
{
    public ExistingElementDTOValidator()
    {
        RuleFor(dto => dto).NotNull();
        RuleFor(req => req.ExistingId)
            .NotNull()
            .NotEmpty()
            .NotEqual(Guid.Empty)
            .WithMessage("ID should be provided for existing element");
    }
}
