using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed record Address
{
    public string Value { get; }
    private Address(string value) => Value = value;

    public static Result<Address> Create(string value) =>
        string.IsNullOrWhiteSpace(value) ? new Error("Address value is empty") : new Address(value);
}
