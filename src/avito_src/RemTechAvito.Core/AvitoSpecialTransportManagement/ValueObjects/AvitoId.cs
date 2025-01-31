using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AvitoId
{
    public ulong Id { get; }

    private AvitoId(ulong id) => Id = id;

    public static Result<AvitoId> Create(string? id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return new Error("Id cannot be empty");
        bool conversion = ulong.TryParse(id, out ulong parsedId);
        if (!conversion)
            return new Error("Id is not ulong value");
        return new AvitoId(parsedId);
    }
}
