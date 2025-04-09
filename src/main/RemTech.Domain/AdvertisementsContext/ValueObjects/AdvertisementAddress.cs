using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementAddress
{
    public const int MAX_ADDRESS_LENGTH = 100;
    public string Value { get; private init; }

    private AdvertisementAddress()
    {
        Value = string.Empty;
    } // ef core.

    private AdvertisementAddress(string address) => Value = address;

    public static Result<AdvertisementAddress> Create(string? address)
    {
        if (string.IsNullOrWhiteSpace(address))
            return ErrorFactory.EmptyOrNull(nameof(AdvertisementAddress));

        if (address.Length > MAX_ADDRESS_LENGTH)
            return ErrorFactory.ExceesLength(nameof(AdvertisementAddress), MAX_ADDRESS_LENGTH);

        return new AdvertisementAddress(address);
    }
}
