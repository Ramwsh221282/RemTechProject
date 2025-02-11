using FluentValidation;

namespace WebDriver.Application.DTO;

public sealed record DriverStartDataDTO(string Strategy);

internal sealed class DriverStartDataDTOValidator : AbstractValidator<DriverStartDataDTO>
{
    private static readonly string[] _allowedStrategies = ["none", "eager", "default", "normal"];

    public DriverStartDataDTOValidator()
    {
        RuleFor(req => req).NotNull().WithMessage("Driver start data is null");
        RuleFor(req => req.Strategy)
            .NotNull()
            .NotEmpty()
            .Must(strategy => _allowedStrategies.Any(s => s == strategy))
            .WithMessage("Invalid page load strategy. (Null, empty or not supported)");
    }
}
