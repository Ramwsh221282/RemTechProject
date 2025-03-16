using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.GetParser;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserExistanceChecker
    : ICommandHandler<UpdateParserCommand, ParserResponse>
{
    private readonly DataServiceMessagerFactory _factory;
    private readonly ICommandHandler<UpdateParserCommand, ParserResponse> _handler;

    public UpdateParserExistanceChecker(
        DataServiceMessagerFactory factory,
        ICommandHandler<UpdateParserCommand, ParserResponse> handler
    )
    {
        _factory = factory;
        _handler = handler;
    }

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand command,
        CancellationToken ct = default
    )
    {
        ParserQueryPayload payload = new(ServiceName: command.NewModel.ParserName);
        GetParserMessage message = new(payload);
        ContractActionResult result = await _factory.Create().Send(message, ct);
        if (!result.IsSuccess)
            return new Error(result.Error);

        return await _handler.Handle(command, ct);
    }
}
