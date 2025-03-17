using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;

public sealed class DeleteParserMapping : ICommandHandler<DeleteParserCommand, Result>
{
    private readonly ICommandHandler<DeleteParserCommand, Result> _handler;
    private readonly DeleteParserContext _context;

    public DeleteParserMapping(
        ICommandHandler<DeleteParserCommand, Result> handler,
        DeleteParserContext context
    )
    {
        _handler = handler;
        _context = context;
    }

    public async Task<Result<Result>> Handle(
        DeleteParserCommand command,
        CancellationToken ct = default
    )
    {
        ParserQueryPayload payload = new ParserQueryPayload(ServiceName: command.ParserName);
        _context.Payload = Option<ParserQueryPayload>.Some(payload);
        return await _handler.Handle(command, ct);
    }
}
