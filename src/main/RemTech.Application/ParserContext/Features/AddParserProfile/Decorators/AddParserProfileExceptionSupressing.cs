using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Shared.SDK.Logging;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;

public sealed class AddParserProfileExceptionSupressing(
    ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> next,
    ILogger logger
) : ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>>
{
    private readonly ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>> _next =
        next;
    private readonly ILogger _logger = logger;

    public async Task<UnitResult<ParserProfile>> Handle(
        AddParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        try
        {
            UnitResult<ParserProfile> result = await _next.Handle(command, ct);
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogFatal(nameof(AddParserProfileCommand), nameof(Handle), ex.Message);
            return UnitResult<ParserProfile>.InternalServerError();
        }
    }
}
