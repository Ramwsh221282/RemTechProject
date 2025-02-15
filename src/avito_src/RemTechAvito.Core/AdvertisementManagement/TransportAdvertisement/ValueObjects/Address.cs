using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record Address
{
    public string Text { get; }

    private Address(string text) => Text = text;

    public static Result<Address> Create(string? text) =>
        string.IsNullOrWhiteSpace(text) ? new Error("Address cannot be empty") : new Address(text);
}
