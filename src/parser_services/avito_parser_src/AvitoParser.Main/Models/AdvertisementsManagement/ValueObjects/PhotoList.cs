using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.Main.Models.AdvertisementsManagement.ValueObjects;

public sealed class PhotoList
{
    private readonly List<Photo> _photos = [];
    private IReadOnlyCollection<Photo> Photos
    {
        get => _photos;
        set
        {
            _photos.Clear();
            _photos.AddRange(value);
        }
    }

    private PhotoList(List<Photo> photos) => _photos = photos;

    public static PhotoList Empty => new PhotoList([]);

    public PhotoList Add(Photo photo)
    {
        List<Photo> copy = _photos;
        copy.Add(photo);
        return new PhotoList(copy);
    }
}

public sealed record Photo
{
    public string SourcePath { get; }

    private Photo(string sourcePath) => SourcePath = sourcePath;

    public static Result<Photo> Create(string? sourcePath)
    {
        if (string.IsNullOrWhiteSpace(sourcePath))
            return new Error("Photo source path is empty");
        return new Photo(sourcePath);
    }
}
