using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.ParsersManagement.Features.GetAllParsers;

public sealed record GetAllParsersMessage : IContract;

public sealed record GetAllParsersQuery : IRequest<Option<ParserResponse[]>>;

public sealed class GetAllParsersQueryHandler(DataServiceMessager messager)
    : IRequestHandler<GetAllParsersQuery, Option<ParserResponse[]>>
{
    private readonly DataServiceMessager _messager = messager;

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
