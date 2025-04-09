namespace SharedParsersLibrary.Models;

public sealed record ScrapedCharacteristic(string Name, string Value)
{
    public static ScrapedCharacteristic Default() =>
        new ScrapedCharacteristic(string.Empty, string.Empty);
}
