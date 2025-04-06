using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.Logging;
using RemTech.Shared.SDK.ResultPattern;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileExceptionSupressing(
    ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> next,
    ILogger logger
) : ICommandHandler<AddParserProfileCommand, UnitResult<Guid>>
{
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<Guid>> _next = next;
    private readonly ILogger _logger = logger;

    public async Task<UnitResult<Guid>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        try
        {
            UnitResult<Guid> result = await _next.Handle(command, ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogFatal(nameof(AddParserProfileCommand), nameof(Handle), ex.Message);
            return UnitResult<Guid>.InternalServerError();
        }
    }
}
