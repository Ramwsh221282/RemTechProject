using Dapper.FluentMap.Mapping;
using RemTech.Application.AdvertisementsContext.Models.CharacteristicsManagement;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.EntityMappings;

public sealed class TransportCharacteristicEntityMap : EntityMap<TransportCharacteristic>
{
    public TransportCharacteristicEntityMap()
    {
        Map(c => c.Id).ToColumn("id");
        Map(c => c.Name).ToColumn("name");
    }
}
