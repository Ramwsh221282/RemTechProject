using RemTech.MainApi.Common.Extensions;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;

public sealed class DeleteParserLogging(
    IRequestHandler<DeleteParserCommand, Result> handler,
    ILogger logger
) : IRequestHandler<DeleteParserCommand, Result>
{
    private readonly ILogger _logger = logger;
    private readonly IRequestHandler<DeleteParserCommand, Result> _handler = handler;

    public async Task<Result> Handle(DeleteParserCommand request, CancellationToken ct = default)
    {
        Result deletion = await _handler.Handle(request, ct);
        return deletion
            .ToWhen()
            .IfFailure(() => _logger.LogError(deletion.Error, nameof(DeleteParserCommand)))
            .IfSuccess(
                () =>
                    _logger.LogFromContext(
                        $"Deleted parser with name: {request.ParserName}",
                        nameof(DeleteParserCommand)
                    )
            )
            .BackToResult();
    }
}
