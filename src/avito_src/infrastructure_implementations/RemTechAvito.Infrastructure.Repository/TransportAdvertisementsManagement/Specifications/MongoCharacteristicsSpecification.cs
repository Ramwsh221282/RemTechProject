using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.Common.Specifications;
using RemTechAvito.Infrastructure.Repository.Specifications;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Specifications;

public class MongoCharacteristicsSpecification
    : SpecificationBase<TransportAdvertisement>,
        IMongoSpecification<TransportAdvertisement>
{
    private readonly CharacteristicsListDto? _dto;

    public MongoCharacteristicsSpecification(CharacteristicsListDto? dto) => _dto = dto;

    public override bool IsSatisfiedBy(TransportAdvertisement entity)
    {
        throw new NotImplementedException();
    }

    public FilterDefinition<TransportAdvertisement> ToFilterDefinition()
    {
        if (_dto == null)
            return Builders<TransportAdvertisement>.Filter.Empty;

        var builder = Builders<TransportAdvertisement>.Filter;
        var text = new List<string>();

        foreach (var ctx in _dto.Characteristics)
        {
            if (!string.IsNullOrWhiteSpace(ctx.Name))
                text.Add(ctx.Name);

            if (!string.IsNullOrWhiteSpace(ctx.Value))
                text.Add(ctx.Value);
        }

        string combinedText = string.Join(" ", text);
        return builder.Text(combinedText);
    }
}
