using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record OwnerInformation
{
    private string Text { get; }
    private string Status { get; }

    public OwnerInformation(string text, string status)
    {
        Text = text;
        Status = status;
    }

    public static Result<OwnerInformation> Create(string? text, string? status)
    {
        if (string.IsNullOrWhiteSpace(text))
            return new Error("Owner information text should be provided");

        if (string.IsNullOrWhiteSpace(status))
            return new Error("Owner status information should be provided");

        return new OwnerInformation(text, status);
    }
}
