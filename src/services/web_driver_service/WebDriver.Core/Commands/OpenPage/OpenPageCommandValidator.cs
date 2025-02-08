using FluentValidation;

namespace RemTech.WebDriver.Plugin.Commands.OpenPage;

internal sealed class OpenPageCommandValidator : AbstractValidator<OpenPageCommand>
{
    public OpenPageCommandValidator()
    {
        RuleFor(com => com.PageUrl).NotNull().NotEmpty().WithMessage("Page url was empty or null");
    }
}
