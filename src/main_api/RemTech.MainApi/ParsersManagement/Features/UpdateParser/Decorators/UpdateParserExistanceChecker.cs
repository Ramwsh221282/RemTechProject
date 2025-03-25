using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.GetParser;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;

public sealed class UpdateParserExistanceChecker(
    DataServiceMessagerFactory factory,
    IRequestHandler<UpdateParserCommand, Result<ParserResponse>> handler
) : IRequestHandler<UpdateParserCommand, Result<ParserResponse>>
{
    private readonly DataServiceMessagerFactory _factory = factory;
    private readonly IRequestHandler<UpdateParserCommand, Result<ParserResponse>> _handler =
        handler;

    public async Task<Result<ParserResponse>> Handle(
        UpdateParserCommand request,
        CancellationToken ct = default
    )
    {
        ParserQueryPayload payload = new(ServiceName: request.NewModel.ParserName);
        GetParserMessage message = new(payload);
        ContractActionResult result = await _factory.Create().Send(message, ct);
        return !result.IsSuccess ? new Error(result.Error) : await _handler.Handle(request, ct);
    }
}
