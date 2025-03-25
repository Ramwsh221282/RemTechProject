using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTechCommon.Utils.OptionPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;

public sealed class DeleteParserMapping(
    IRequestHandler<DeleteParserCommand, Result> handler,
    DeleteParserContext context
) : IRequestHandler<DeleteParserCommand, Result>
{
    private readonly IRequestHandler<DeleteParserCommand, Result> _handler = handler;
    private readonly DeleteParserContext _context = context;

    public async Task<Result> Handle(DeleteParserCommand request, CancellationToken ct = default)
    {
        ParserQueryPayload payload = new ParserQueryPayload(ServiceName: request.ParserName);
        _context.Payload = Option<ParserQueryPayload>.Some(payload);
        return await _handler.Handle(request, ct);
    }
}
