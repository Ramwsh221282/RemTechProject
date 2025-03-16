using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Extensions;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;

public sealed class CreateParserLogging : ICommandHandler<CreateParserCommand, Parser>
{
    private readonly ICommandHandler<CreateParserCommand, Parser> _handler;
    private readonly ILogger _logger;

    public CreateParserLogging(ICommandHandler<CreateParserCommand, Parser> handler, ILogger logger)
    {
        _handler = handler;
        _logger = logger;
    }

    public async Task<Result<Parser>> Handle(
        CreateParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<Parser> result = await _handler.Handle(command, ct);
        if (result.IsFailure)
        {
            _logger.LogError(result.Error, nameof(CreateParserCommand));
            return result;
        }

        _logger.LogFromContext(
            $"Created parser with name: {command.ParserName}",
            nameof(CreateParserCommand)
        );
        return result;
    }
}
