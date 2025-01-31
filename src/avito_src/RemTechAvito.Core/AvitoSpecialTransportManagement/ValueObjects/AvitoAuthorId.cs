using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AvitoAuthorId
{
    public ulong Id { get; }

    private AvitoAuthorId(ulong id) => Id = id;

    public static Result<AvitoAuthorId> Create(string? id)
    {
        Result<ulong> value = StringToUlongConverter.Convert(id);
        return value.IsSuccess ? new AvitoAuthorId(value) : value.Error;
    }
}
