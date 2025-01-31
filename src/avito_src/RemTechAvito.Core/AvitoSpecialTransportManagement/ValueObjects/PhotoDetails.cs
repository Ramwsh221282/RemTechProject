using RemTechCommon;

namespace RemTechAvito.Core.AvitoSpecialTransportManagement.ValueObjects;

public sealed record PhotoDetails
{
    private readonly List<Photo> _photos = [];
    public IReadOnlyCollection<Photo> Photos => _photos;

    private PhotoDetails() { } // ef core

    public PhotoDetails(IEnumerable<Photo> photos)
    {
        _photos = photos.ToList();
    }

    public static PhotoDetails Empty() => new();
}

public sealed record Photo
{
    public string Url { get; }

    private Photo(string url) => Url = url;

    public static Result<Photo> Create(string? url)
    {
        if (string.IsNullOrWhiteSpace(url))
            return new Error("Url is required");
        return new Photo(url);
    }
}
