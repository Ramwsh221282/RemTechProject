using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetParser;

public sealed record GetParserMessage(ParserQueryPayload Payload) : IContract;

public sealed record GetParserQuery(string Name) : IRequest<Option<ParserResponse>>;

public sealed class GetParserQueryHandler(DataServiceMessagerFactory factory)
    : IRequestHandler<GetParserQuery, Option<ParserResponse>>
{
    private readonly DataServiceMessagerFactory _factory = factory;

    public async Task<Option<ParserResponse>> Handle(
        GetParserQuery request,
        CancellationToken ct = default
    )
    {
        ParserQueryPayload payload = new ParserQueryPayload(ServiceName: request.Name);
        DataServiceMessager messager = _factory.Create();
        ContractActionResult result = await messager.Send(new GetParserMessage(payload), ct);
        return result.IsSuccess switch
        {
            true => result.FromResult<ParserDaoResponse>().MapToParser().ToResponse(),
            false => Option<ParserResponse>.None(),
        };
    }
}
