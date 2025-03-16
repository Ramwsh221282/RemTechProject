using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;

public sealed class DeleteParserLogging : ICommandHandler<DeleteParserCommand, Result>
{
    private readonly ILogger _logger;
    private readonly ICommandHandler<DeleteParserCommand, Result> _handler;

    public DeleteParserLogging(ICommandHandler<DeleteParserCommand, Result> handler, ILogger logger)
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task<Result<Result>> Handle(
        DeleteParserCommand command,
        CancellationToken ct = default
    )
    {
        Result deletion = await _handler.Handle(command, ct);
        if (deletion.IsFailure)
        {
            _logger.LogError(deletion.Error, nameof(DeleteParserCommand));
            return deletion;
        }

        _logger.LogFromContext(
            $"Deleted parser with name: {command.ParserName}",
            nameof(DeleteParserCommand)
        );
        return deletion;
    }
}
