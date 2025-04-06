using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.Logging;
using RemTech.Shared.SDK.ResultPattern;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileLogging(
    ILogger logger,
    ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> next
) : ICommandHandler<AddParserProfileCommand, UnitResult<Guid>>
{
    private readonly ILogger _logger = logger;
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> _next = next;

    public async Task<UnitResult<Guid>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        UnitResult<Guid> result = await _next.Handle(command, ct);

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
