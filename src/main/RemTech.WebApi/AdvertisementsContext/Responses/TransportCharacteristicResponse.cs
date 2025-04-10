using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

namespace RemTech.WebApi.AdvertisementsContext.Responses;

public sealed record TransportCharacteristicResponse(Guid Id, string Name);

public static class TransportCharacteristicResponseExtensions
{
    public static TransportCharacteristicResponse ToResponse(this TransportCharacteristicDao dao)
    {
        return new TransportCharacteristicResponse(dao.Id, dao.Name);
    }
}
