using MongoDB.Driver;
using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Dtos;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Extensions;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed record TransportAdvertisementDaoResponse(
    TransportAdvertisement[] Advertisements,
    long Count
);

public sealed record GetAdvertisementsMessage(AdvertisementsQuery Query) : IContract;

public sealed class GetAdvertisementsMessageHandler(AdvertisementsRepository repository)
    : IContractHandler<GetAdvertisementsMessage>
{
    private readonly AdvertisementsRepository _repository = repository;
    private readonly IQueryBuilder<AdvertisementQueryPayload, Advertisement> _builder =
        new AdvertisementQueryBuilder();

    public async Task<ContractActionResult> Handle(GetAdvertisementsMessage contract)
    {
        AdvertisementQueryPayload payload = contract.Query.ResolveQueryPayload();
        PaginationOption pagination = contract.Query.Pagination;
        _builder.SetPayload(payload);

        FilterDefinition<Advertisement> filter = _builder.Build();
        var queryDataTask = _repository.GetMany(filter, pagination);
        var countDataTask = _repository.GetCount(filter);
        await Task.WhenAll(queryDataTask, countDataTask);

        TransportAdvertisement[] queryDataResult = queryDataTask
            .Result.AsChunk()
            .Map(AdvertisementExtensions.ToTransportAdvertisement)
            .AsArray();
        long count = countDataTask.Result;
        TransportAdvertisementDaoResponse response = new(queryDataResult, count);

        return response.Success();
    }
}
