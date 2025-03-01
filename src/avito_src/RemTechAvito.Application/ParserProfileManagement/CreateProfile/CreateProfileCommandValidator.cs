using FluentValidation;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechCommon.Injections;

namespace RemTechAvito.Application.ParserProfileManagement.CreateProfile;

internal sealed class CreateProfileCommandValidator : AbstractValidator<CreateProfileCommand>
{
    public CreateProfileCommandValidator()
    {
        RuleFor(x => x.Name).MustBeSuccessResult(ParserProfileName.Create);
    }
}
