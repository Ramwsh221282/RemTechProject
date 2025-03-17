using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetAllParsers;

public sealed record GetAllParsersMessage : IContract;

public sealed record GetAllParsersQuery : IQuery<ParserResponse[]>;

public sealed class GetAllParsersQueryHandler : IQueryHandler<GetAllParsersQuery, ParserResponse[]>
{
    private readonly DataServiceMessager _messager;

    public GetAllParsersQueryHandler(DataServiceMessager messager) => _messager = messager;

    public async Task<Option<ParserResponse[]>> Handle(
        GetAllParsersQuery query,
        CancellationToken ct = default
    ) =>
        Option<ParserResponse[]>.Some(
            (await _messager.Send(new GetAllParsersMessage(), ct))
                .FromResult<ParserDaoResponse[]>()
                .Select(d => d.MapToParser().ToResponse())
                .ToArray()
        );
}
