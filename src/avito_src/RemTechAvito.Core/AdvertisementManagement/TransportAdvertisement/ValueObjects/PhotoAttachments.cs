using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;

public sealed record PhotoAttachments
{
    private readonly List<Photo> _photos = [];
    public IReadOnlyCollection<Photo> Photos => _photos;

    public PhotoAttachments(List<Photo> photos)
    {
        _photos = photos;
    }
}

public sealed record Photo
{
    public string Path { get; }

    private Photo(string path) => Path = path;

    public static Result<Photo> Create(string? path) =>
        string.IsNullOrWhiteSpace(path)
            ? new Error("Photo path should be provided")
            : new Photo(path);
}
