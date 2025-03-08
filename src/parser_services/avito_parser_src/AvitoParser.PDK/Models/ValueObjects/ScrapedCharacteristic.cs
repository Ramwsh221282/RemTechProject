using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK.Models.ValueObjects;

public sealed record ScrapedCharacteristic
{
    public string Name { get; }
    public string Value { get; }

    private ScrapedCharacteristic(string name, string value)
    {
        Name = name;
        Value = value;
    }

    public static Result<ScrapedCharacteristic> Create(string? name, string? value)
    {
        if (string.IsNullOrWhiteSpace(name))
            return new Error("Characteristic name is empty");
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Characteristic value is empty");
        return new ScrapedCharacteristic(name, value);
    }
}
