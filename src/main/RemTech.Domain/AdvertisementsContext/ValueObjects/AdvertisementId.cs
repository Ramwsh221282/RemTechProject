using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public struct AdvertisementId
{
    public long Value;

    public AdvertisementId() => Value = 0;

    private AdvertisementId(long value) => Value = value;

    public static AdvertisementId Empty() => new AdvertisementId();

    public static Result<AdvertisementId> Dedicated(long value)
    {
        return value <= 0
            ? new Error($"Некорректный ID объявления - {value}.")
            : new AdvertisementId(value);
    }
}
