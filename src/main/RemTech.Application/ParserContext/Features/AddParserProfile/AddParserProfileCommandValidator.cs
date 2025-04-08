using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Domain.ParserContext.ValueObjects;

namespace RemTech.Application.ParserContext.Features.AddParserProfile;

public sealed class AddParserProfileCommandValidator : IValidator<AddParserProfileCommand>
{
    public ValidationResult Validate(AddParserProfileCommand validatee)
    {
        (string parserName, string profileName) = validatee;

        Result<ParserName> parserNameRes = ParserName.Create(parserName);
        if (parserNameRes.IsFailure)
            return ValidationResult.FromErrorResult(parserNameRes);

        Result<ParserProfileName> profileNameRes = ParserProfileName.Create(profileName);
        if (profileNameRes.IsFailure)
            return ValidationResult.FromErrorResult(profileNameRes);

        return ValidationResult.Success;
    }
}
