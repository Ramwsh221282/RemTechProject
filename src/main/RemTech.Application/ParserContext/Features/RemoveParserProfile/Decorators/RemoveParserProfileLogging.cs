using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.Logging;
using RemTech.Shared.SDK.ResultPattern;
using Serilog;

namespace RemTech.Application.ParserContext.Features.RemoveParserProfile.Decorators;

public sealed class RemoveParserProfileLogging(
    ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>> next,
    ILogger logger
) : ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>>
{
    private readonly ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>> _next = next;
    private readonly ILogger _logger = logger;

    public async Task<UnitResult<Guid>> Handle(
        RemoveParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UnitResult<Guid> result = await _next.Handle(command, ct);
        if (result.IsFailure)
            _logger.LogError(nameof(RemoveParserProfileCommand), nameof(Handle), result.Message);

        if (result.IsSuccess)
            _logger.LogInfo(
                nameof(RemoveParserProfileCommand),
                nameof(Handle),
                $"Remove parser profile {command.ProfileName} of parser: {command.ParserName}"
            );

        return result;
    }
}
