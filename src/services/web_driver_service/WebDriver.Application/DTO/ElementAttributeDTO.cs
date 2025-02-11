using FluentValidation;

namespace WebDriver.Application.DTO;

public sealed record ElementAttributeDTO(string Attribute);

internal sealed class ElementAttributeDTOValidator : AbstractValidator<ElementAttributeDTO>
{
    public ElementAttributeDTOValidator()
    {
        RuleFor(req => req).NotNull().WithMessage("Request body should not be null");
        RuleFor(req => req.Attribute)
            .NotNull()
            .NotEmpty()
            .WithMessage("Attribute name should be provided");
    }
}
