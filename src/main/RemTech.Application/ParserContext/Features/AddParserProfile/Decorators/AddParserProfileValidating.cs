using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Validators;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileValidating(
    IValidator<AddParserProfileCommand> validator,
    ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> next
) : ICommandHandler<AddParserProfileCommand, UnitResult<Guid>>
{
    private readonly IValidator<AddParserProfileCommand> _validator = validator;
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> _next = next;

    public async Task<UnitResult<Guid>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = _validator.Validate(command);
        return !validation.IsValid
            ? validation.FromValidationFailure<Guid>()
            : await _next.Handle(command, ct);
    }
}
