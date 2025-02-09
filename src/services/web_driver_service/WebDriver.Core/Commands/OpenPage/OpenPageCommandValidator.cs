using FluentValidation;

namespace WebDriver.Core.Commands.OpenPage;

public sealed class OpenPageCommandValidator : AbstractValidator<OpenPageCommand>
{
    public OpenPageCommandValidator()
    {
        RuleFor(com => com.PageUrl).NotNull().NotEmpty().WithMessage("Page url was empty or null");
    }
}
