using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Queries.FilterAdvertisements.Specifications;

public sealed class DateSpecification : SpecificationBase<TransportAdvertisement>
{
    private readonly DateFilterDto? _date;

    public DateSpecification(DateFilterDto? date) => _date = date;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        if (_date == null)
            return true;

        var createdDate = entity.CreatedOn;

        return _date.Predicate switch
        {
            "LESS" => createdDate < _date.Date,
            "MORE" => createdDate > _date.Date,
            "EQUAL" => createdDate.Equals(_date.Date),
            _ => true,
        };
    }
}
