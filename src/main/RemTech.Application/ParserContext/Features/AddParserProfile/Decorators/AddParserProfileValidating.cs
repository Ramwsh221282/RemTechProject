using RemTech.Domain.ParserContext.Entities.ParserProfiles;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileValidating(
    IValidator<AddParserProfileCommand> validator,
    ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> next
) : ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>>
{
    private readonly IValidator<AddParserProfileCommand> _validator = validator;
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> _next =
        next;

    public async Task<UnitResult<ParserProfile>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = _validator.Validate(command);
        return !validation.IsValid
            ? validation.FromValidationFailure<ParserProfile>()
            : await _next.Handle(command, ct);
    }
}
