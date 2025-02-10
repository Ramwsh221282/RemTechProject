using FluentValidation;

namespace WebDriver.Application.DTO;

public sealed record ElementPathDataDTO(string Path, string Type);

internal sealed class ElementPathDataDTOValidator : AbstractValidator<ElementPathDataDTO>
{
    public static readonly string[] AllowedPaths = ["xpath", "class", "tag"];

    public ElementPathDataDTOValidator()
    {
        RuleFor(dto => dto).NotNull();
        RuleFor(req => req.Path)
            .NotNull()
            .NotEmpty()
            .WithMessage("Element search path should be provided");
        RuleFor(req => req.Type)
            .NotNull()
            .NotEmpty()
            .Must(type => AllowedPaths.Any(p => p == type))
            .WithMessage("Type is not provided or not supported.");
    }
}
