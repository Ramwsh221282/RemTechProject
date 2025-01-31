using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record AddressInfo
{
    public string Text { get; }

    private AddressInfo(string text) => Text = text;

    public static Result<AddressInfo> Create(string? text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Address info is empty");
        return new AddressInfo(text);
    }
}
