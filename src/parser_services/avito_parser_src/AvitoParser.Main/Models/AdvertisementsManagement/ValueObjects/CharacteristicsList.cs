using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record CharacteristicsList
{
    private readonly List<Characteristic> _characteristics = [];
    public IReadOnlyCollection<Characteristic> Characteristics
    {
        get => _characteristics;
        set
        {
            _characteristics.Clear();
            _characteristics.AddRange(value);
        }
    }

    public static CharacteristicsList Empty => new CharacteristicsList([]);

    public CharacteristicsList(List<Characteristic> characteristics) =>
        _characteristics = characteristics;

    public CharacteristicsList Add(Characteristic characteristic)
    {
        List<Characteristic> copy = _characteristics;
        copy.Add(characteristic);
        return new CharacteristicsList(copy);
    }
}

public sealed record Characteristic
{
    public string Name { get; }
    public string Value { get; }

    private Characteristic(string name, string value) => (Name, Value) = (name, value);

    public static Result<Characteristic> Create(string? name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Characteristic name is empty");
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Characteristic value is empty");
        return new Characteristic(name, value);
    }
}
