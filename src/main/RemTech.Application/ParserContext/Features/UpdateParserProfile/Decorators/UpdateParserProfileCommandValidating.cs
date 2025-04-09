using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Validators;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile.Decorators;

public sealed class UpdateParserProfileCommandValidating(
    ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> handler,
    IValidator<UpdateParserProfileCommand> validator
) : ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>>
{
    private readonly ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> _handler =
        handler;
    private readonly IValidator<UpdateParserProfileCommand> _validator = validator;

    public async Task<UnitResult<Guid>> Handle(
        UpdateParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        ValidationResult validation = _validator.Validate(command);
        return validation.IsValid == false
            ? validation.FromValidationFailure<Guid>()
            : await _handler.Handle(command, ct);
    }
}
