using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Extensions;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserLogging : ICommandHandler<UpdateParserCommand, ParserResponse>
{
    private readonly ILogger _logger;
    private readonly ICommandHandler<UpdateParserCommand, ParserResponse> _handler;

    public UpdateParserLogging(
        ILogger logger,
        ICommandHandler<UpdateParserCommand, ParserResponse> handler
    )
    {
        _logger = logger;
        _handler = handler;
    }

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand command,
        CancellationToken ct = default
    )
    {
        Result<ParserResponse> result = await _handler.Handle(command, ct);
        if (result.IsFailure)
            _logger.LogError(result.Error, nameof(UpdateParserCommand));
        else
            _logger.LogFromContext(
                $"Parser Configuration of {command.NewModel.ParserName}  has been updated.",
                nameof(UpdateParserCommand)
            );

        return result;
    }
}
