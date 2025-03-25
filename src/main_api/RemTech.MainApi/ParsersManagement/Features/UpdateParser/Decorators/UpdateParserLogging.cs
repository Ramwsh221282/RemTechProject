using RemTech.MainApi.Common.Extensions;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserLogging(
    IRequestHandler<UpdateParserCommand, Result<ParserResponse>> handler,
    ILogger logger
) : IRequestHandler<UpdateParserCommand, Result<ParserResponse>>
{
    private readonly ILogger _logger = logger;
    private readonly IRequestHandler<UpdateParserCommand, Result<ParserResponse>> _handler =
        handler;

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<ParserResponse> result = await _handler.Handle(command, ct);
        return result
            .ToWhen()
            .IfFailure(() => _logger.LogError(result.Error, nameof(UpdateParserCommand)))
            .IfSuccess(
                () =>
                    _logger.LogFromContext(
                        $"Parser Configuration of {command.NewModel.ParserName} has been updated.",
                        nameof(UpdateParserCommand)
                    )
            )
            .BackToResult();
    }
}
