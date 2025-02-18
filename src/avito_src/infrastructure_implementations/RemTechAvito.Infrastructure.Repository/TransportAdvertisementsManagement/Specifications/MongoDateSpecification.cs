using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public sealed class MongoDateSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly DateFilterDto? _date;

    public MongoDateSpecification(DateFilterDto date) => _date = date;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        if (_date == null)
            return Builders<TransportAdvertisement>.Filter.Empty;

        var builder = Builders<TransportAdvertisement>.Filter;
        return _date.Predicate switch
        {
            "LESS" => builder.Lt(ad => ad.CreatedOn.ToUnix(), _date.Date.ToUnix()),
            "MORE" => builder.Gt(ad => ad.CreatedOn.ToUnix(), _date.Date.ToUnix()),
            "EQUAL" => builder.Eq(ad => ad.CreatedOn.ToUnix(), _date.Date.ToUnix()),
            _ => builder.Empty,
        };
    }
}
