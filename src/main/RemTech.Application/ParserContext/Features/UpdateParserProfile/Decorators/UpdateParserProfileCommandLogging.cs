using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.Logging;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Validators;
using Serilog;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile.Decorators;

public sealed class UpdateParserProfileCommandLogging(
    ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> next,
    ILogger logger
) : ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>>
{
    private readonly ILogger _logger = logger;
    private readonly ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> _next = next;

    public async Task<UnitResult<Guid>> Handle(
        UpdateParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UnitResult<Guid> result = await _next.Handle(command, ct);
        if (result.IsFailure)
            _logger.LogError(nameof(UpdateParserProfileCommand), nameof(Handle), result.Message);

        if (result.IsSuccess)
            _logger.LogInfo(
                nameof(UpdateParserProfileCommand),
                nameof(Handle),
                $"Updated profile with id: {result.Result} of parser: {command.ParserName}"
            );

        return result;
    }
}
