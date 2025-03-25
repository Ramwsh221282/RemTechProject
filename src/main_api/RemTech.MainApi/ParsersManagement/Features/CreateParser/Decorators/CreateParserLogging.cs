using RemTech.MainApi.Common.Extensions;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;

public sealed class CreateParserLogging(
    IRequestHandler<CreateParserCommand, Result<Parser>> handler,
    ILogger logger
) : IRequestHandler<CreateParserCommand, Result<Parser>>
{
    private readonly IRequestHandler<CreateParserCommand, Result<Parser>> _handler = handler;
    private readonly ILogger _logger = logger;

    public async Task<Result<Parser>> Handle(
        CreateParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<Parser> result = await _handler.Handle(command, ct);
        return result
            .ToWhen()
            .IfFailure(() => _logger.LogError(result.Error, nameof(CreateParserCommand)))
            .IfSuccess(
                () =>
                    _logger.LogFromContext(
                        $"Created parser with name: {command.ParserName}",
                        nameof(CreateParserCommand)
                    )
            )
            .BackToResult();
    }
}
