using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public sealed class MongoAdvertisementSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly FilterAdvertisementsDto _dto;

    public MongoAdvertisementSpecification(FilterAdvertisementsDto dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        var builder = Builders<TransportAdvertisement>.Filter;
        List<FilterDefinition<TransportAdvertisement>> filters = [];

        if (_dto.Address != null)
            filters.Add(new MongoAddressSpecification(_dto.Address).ToFilterDefinition());

        if (_dto.Characteristics != null)
            filters.Add(
                new MongoCharacteristicsSpecification(_dto.Characteristics).ToFilterDefinition()
            );

        if (_dto.Date != null)
            filters.Add(new MongoDateSpecification(_dto.Date).ToFilterDefinition());

        if (_dto.Description != null)
            filters.Add(new MongoDescriptionSpecification(_dto.Description).ToFilterDefinition());

        if (_dto.Price != null)
            filters.Add(new MongoPriceSpecification(_dto.Price).ToFilterDefinition());

        if (_dto.OwnerInformation != null)
            filters.Add(new MongoOwnerSpecification(_dto.OwnerInformation).ToFilterDefinition());

        return builder.And(filters);
    }
}
