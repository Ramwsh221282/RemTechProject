using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record Description
{
    public string Value { get; }
    
    private Description(string value) => Value = value;

    public static Result<Description> Create(string? value) =>
        string.IsNullOrWhiteSpace(value) ? new Error("Description cannot be empty") : new Description(value);
}