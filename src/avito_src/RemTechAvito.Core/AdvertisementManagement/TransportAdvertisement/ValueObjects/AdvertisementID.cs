using RemTechCommon.Utils.Converters;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record AdvertisementID
{
    public uint Id { get; }

    private AdvertisementID(uint id) => Id = id;

    public static AdvertisementID Create(uint id) => new AdvertisementID(id);

    public static Result<AdvertisementID> Create(string? id)
    {
        Result<uint> converted = id.ToUint();
        return converted.IsFailure ? converted.Error : Create(converted);
    }
}
