using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Shared.SDK.Logging;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileLogging(
    ILogger logger,
    ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> next
) : ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>>
{
    private readonly ILogger _logger = logger;
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> _next =
        next;

    public async Task<UnitResult<ParserProfile>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UnitResult<ParserProfile> result = await _next.Handle(command, ct);

        if (result.IsFailure)
            _logger.LogError(nameof(AddParserProfileCommand), nameof(Handle), result.Message);

        if (result.IsSuccess)
            _logger.LogInfo(
                nameof(AddParserProfileCommand),
                nameof(Handle),
                $"Added profile with ID: {result.Result}"
            );

        return result;
    }
}
