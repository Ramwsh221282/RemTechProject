using RemTechCommon.Utils.Converters;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public readonly record struct AdvertisementID
{
    public long Id { get; }

    private AdvertisementID(long id) => Id = id;

    public static AdvertisementID Create(long id) => new AdvertisementID(id);

    public static Result<AdvertisementID> Create(string? id)
    {
        Result<long> converted = id.ToLong();
        return converted.IsFailure ? converted.Error : Create(converted);
    }
}
