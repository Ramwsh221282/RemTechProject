using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.Sinking;

public sealed class SinkingCharacteristics
{
    public string Name { get; }
    public string Value { get; }

    public SinkingCharacteristics(ScrapedCharacteristic characteristic)
    {
        Name = characteristic.Name;
        Value = characteristic.Value;
    }
}
