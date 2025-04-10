using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.Logging;
using RemTech.Shared.SDK.ResultPattern;
using Serilog;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile.Decorators;

public sealed class UpdateParserProfileCommandException(
    ILogger logger,
    ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> handler
) : ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>>
{
    private readonly ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>> _handler =
        handler;

    public async Task<UnitResult<Guid>> Handle(
        UpdateParserProfileCommand command,
        CancellationToken ct = default
    )
    {
        try
        {
            UnitResult<Guid> result = await _handler.Handle(command, ct);
            return result;
        }
        catch (Exception ex)
        {
            logger.LogFatal(nameof(UpdateParserProfileCommand), nameof(Handle), ex.Message);
            return UnitResult<Guid>.InternalServerError();
        }
    }
}
