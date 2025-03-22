using MongoDB.Driver;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Dtos;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Extensions;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed record GetAdvertisementsMessage(AdvertisementsQuery Query) : IContract;

public sealed class GetAdvertisementsMessageHandler(AdvertisementsRepository repository)
    : IContractHandler<GetAdvertisementsMessage>
{
    private readonly AdvertisementsRepository _repository = repository;
    private readonly IQueryBuilder<AdvertisementQueryPayload, Advertisement> _builder =
        new AdvertisementQueryBuilder();

    public async Task<ContractActionResult> Handle(GetAdvertisementsMessage contract)
    {
        AdvertisementsQuery query = contract.Query;
        AdvertisementQueryPayload payload = query.ResolveQueryPayload();
        PaginationOption pagination = query.Pagination;
        _builder.SetPayload(payload);
        FilterDefinition<Advertisement> filter = _builder.Build();
        return (await _repository.GetMany(filter, pagination))
            .AsChunk()
            .Map(AdvertisementExtensions.ToTransportAdvertisement)
            .AsArray()
            .Success();
    }
}
