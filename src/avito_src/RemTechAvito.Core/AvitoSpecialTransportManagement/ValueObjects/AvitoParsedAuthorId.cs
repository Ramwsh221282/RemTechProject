using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AvitoParsedAuthorId
{
    public ulong Id { get; }

    private AvitoParsedAuthorId(ulong id) => Id = id;

    public static Result<AvitoParsedAuthorId> Create(string? id)
    {
        Result<ulong> value = StringToUlongConverter.Convert(id);
        return value.IsSuccess ? new AvitoParsedAuthorId(value) : value.Error;
    }
}
