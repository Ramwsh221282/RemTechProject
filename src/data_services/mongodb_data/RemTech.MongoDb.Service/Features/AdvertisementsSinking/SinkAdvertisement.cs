using Rabbit.RPC.Server.Abstractions.Communication;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement.Converters;
using RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Features.AddCharacteristic;
using RemTechCommon.Utils.CqrsPattern;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MongoDb.Service.Features.AdvertisementsSinking;

public sealed record SinkAdvertisement(SinkingAdvertisement Advertisement) : IContract;

public sealed class SinkAdvertisementHandler(
    ILogger logger,
    AdvertisementsRepository repository,
    IQueryBuilder<AdvertisementQueryPayload, Advertisement> queryBuilder,
    IRequestHandler<AddCharacteristicRequest, Result> addCharacteristics
) : IContractHandler<SinkAdvertisement>
{
    private readonly ILogger _logger = logger;
    private readonly AdvertisementsRepository _repository = repository;
    private readonly IQueryBuilder<AdvertisementQueryPayload, Advertisement> _queryBuilder =
        queryBuilder;
    private readonly IRequestHandler<AddCharacteristicRequest, Result> _addCharacteristics =
        addCharacteristics;
    private readonly SinkingAdvertisementValidator _validator = new SinkingAdvertisementValidator();
    private readonly AdvertisementsFactory _factory = new AdvertisementsFactory();

    public async Task<ContractActionResult> Handle(SinkAdvertisement contract)
    {
        if (!_validator.Validate(contract.Advertisement))
            return ContractActionResult.Fail("SinkAdvertisement validation failed");

        var advertisement = _factory.FromSinkedAdvertisement(contract.Advertisement);
        var payload = new AdvertisementQueryPayload(
            AdvertisementId: advertisement.AdvertisementId,
            ServiceName: advertisement.ServiceName
        );
        payload = payload.ResolveQueryPayload();
        _queryBuilder.SetPayload(payload);
        var filter = _queryBuilder.Build();

        if (await _repository.Contains(filter))
        {
            _logger.Error(
                "{Context}. Already contains advertisement with ID: {ID} and Service: {ServiceName}",
                nameof(SinkAdvertisement),
                advertisement.AdvertisementId,
                advertisement.ServiceName
            );
            return ContractActionResult.Fail(
                $"Advertisement with ID:{advertisement.AdvertisementId} Service:{advertisement.ServiceName} already exists"
            );
        }

        await _repository.Save(advertisement);
        foreach (var ctx in advertisement.Characteristics)
        {
            AddCharacteristicRequest addCtx = new(new(ctx.Name));
            await _addCharacteristics.Handle(addCtx);
        }

        _logger.Information(
            "Advertisement with ID: {Id} has been added from Parser: {Parser}",
            advertisement.AdvertisementId,
            contract.Advertisement.FromParser
        );

        return ContractActionResult.Success(true);
    }
}
