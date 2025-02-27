using FluentValidation;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechCommon.Injections;

namespace RemTechAvito.Application.ParserProfileManagement.CreateProfile;

internal sealed class ProfileNameDtoValidator : AbstractValidator<ProfileNameDto>
{
    public ProfileNameDtoValidator()
    {
        RuleFor(x => x.Name).MustBeSuccessResult(ParserProfileName.Create);
    }
}
