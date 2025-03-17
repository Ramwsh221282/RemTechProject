using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Features.Shared;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetParser;

public sealed record GetParserMessage(ParserQueryPayload Payload) : IContract;

public sealed record GetParserQuery(string Name) : IQuery<ParserResponse>;

public sealed record GetParserQueryHandler : IQueryHandler<GetParserQuery, ParserResponse>
{
    private readonly DataServiceMessagerFactory _factory;

    public GetParserQueryHandler(DataServiceMessagerFactory factory) => _factory = factory;

    public async Task<Option<ParserResponse>> Handle(
        GetParserQuery query,
        CancellationToken ct = default
    )
    {
        ParserQueryPayload payload = new ParserQueryPayload(ServiceName: query.Name);
        DataServiceMessager messager = _factory.Create();
        ContractActionResult result = await messager.Send(new GetParserMessage(payload), ct);
        return result.IsSuccess switch
        {
            true => Option<ParserResponse>.Some(
                result.FromResult<ParserDaoResponse>().MapToParser().ToResponse()
            ),
            false => Option<ParserResponse>.None(),
        };
    }
}
