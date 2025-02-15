using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record Characteristics
{
    private readonly List<Characteristic> _data = [];
    public IReadOnlyCollection<Characteristic> Data => _data;

    public Characteristics(List<Characteristic> data) => _data = data;
}

public sealed record Characteristic
{
    public string Name { get; }
    public string Value { get; }

    private Characteristic(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static Result<Characteristic> Create(string? name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Characteristic name should be provided");

        if (string.IsNullOrWhiteSpace(value))
            return new Error("Characteristic value should be provided");

        return new Characteristic(name, value);
    }
}
