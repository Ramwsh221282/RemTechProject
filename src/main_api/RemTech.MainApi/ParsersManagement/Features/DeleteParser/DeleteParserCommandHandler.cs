using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser;

public sealed record DeleteParserMessage(ParserQueryPayload Payload) : IContract;

public sealed class DeleteParserCommandHandler : ICommandHandler<DeleteParserCommand, Result>
{
    private readonly DeleteParserContext _context;
    private readonly DataServiceMessager _messager;

    public DeleteParserCommandHandler(DeleteParserContext context, DataServiceMessager messager)
    {
        _context = context;
        _messager = messager;
    }

    public async Task<Result<Result>> Handle(
        DeleteParserCommand command,
        CancellationToken ct = default
    )
    {
        if (!_context.Payload.HasValue)
            return new Error("Parser was not deleted. Internal server error.");

        ParserQueryPayload payload = _context.Payload.Value;
        DeleteParserMessage message = new DeleteParserMessage(payload);
        ContractActionResult result = await _messager.Send(message, ct);
        return result.IsSuccess ? Result.Success() : new Error(result.Error);
    }
}
