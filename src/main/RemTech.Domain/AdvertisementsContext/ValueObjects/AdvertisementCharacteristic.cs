using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementCharacteristic
{
    public const int MAX_CHARACTERISTIC_LENGTH = 30;
    public string Name { get; }
    public string Value { get; }

    private AdvertisementCharacteristic(string name, string value) => (Name, Value) = (name, value);

    public static Result<AdvertisementCharacteristic> Create(string? name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(value))
            return ErrorFactory.EmptyOrNull(nameof(AdvertisementCharacteristic));

        if (name.Length > MAX_CHARACTERISTIC_LENGTH || value.Length > MAX_CHARACTERISTIC_LENGTH)
            return ErrorFactory.ExceesLength(
                nameof(AdvertisementCharacteristic),
                MAX_CHARACTERISTIC_LENGTH
            );

        return new AdvertisementCharacteristic(name, value);
    }
}
