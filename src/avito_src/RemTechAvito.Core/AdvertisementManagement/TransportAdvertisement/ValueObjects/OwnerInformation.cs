using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record OwnerInformation
{
    public string Text { get; }
    public string Status { get; }
    public string Contacts { get; } = string.Empty;

    public OwnerInformation(string text, string status, string? contacts = null)
    {
        Text = text;
        Status = status;
        if (!string.IsNullOrWhiteSpace(contacts))
            Contacts = contacts;
    }

    public static Result<OwnerInformation> Create(
        string? text,
        string? status,
        string? contacts = null
    )
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Owner information text should be provided");

        if (string.IsNullOrWhiteSpace(status))
            return new Error("Owner status information should be provided");

        return new OwnerInformation(text, status, contacts);
    }
}
