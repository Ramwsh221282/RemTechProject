using FluentValidation;

namespace WebDriver.Application.DTO;

public sealed record WebPageDataDTO(string PageUrl);

internal sealed class WebPageDataDTOValidator : AbstractValidator<WebPageDataDTO>
{
    public WebPageDataDTOValidator()
    {
        RuleFor(req => req).NotNull().WithMessage("DTO data should not be null");
        RuleFor(req => req.PageUrl)
            .NotNull()
            .NotEmpty()
            .WithMessage("Web Page Url should be provided");
    }
}
