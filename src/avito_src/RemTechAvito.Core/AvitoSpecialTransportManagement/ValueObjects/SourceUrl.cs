using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record SourceUrl
{
    public string Url { get; }

    private SourceUrl(string url) => Url = url;

    public static Result<SourceUrl> Create(string? url) =>
        string.IsNullOrWhiteSpace(url)
            ? new Error("Source url cannot be empty")
            : new SourceUrl(url);
}
