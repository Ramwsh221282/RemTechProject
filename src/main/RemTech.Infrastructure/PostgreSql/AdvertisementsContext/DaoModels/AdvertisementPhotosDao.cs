using System.Collections;
using System.Text.Json;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

public sealed class AdvertisementPhotosDao : IReadOnlyList<AdvertisementPhotoDao>
{
    private readonly List<AdvertisementPhotoDao> _photos;

    private AdvertisementPhotosDao(AdvertisementPhotoDao[] photos) => _photos = [.. photos];

    public static AdvertisementPhotosDao Create(AdvertisementDao advertisement)
    {
        string json = advertisement.PhotosJson;

        return string.IsNullOrWhiteSpace(json)
            ? new AdvertisementPhotosDao([])
            : new AdvertisementPhotosDao(ParseJson(json));
    }

    private static AdvertisementPhotoDao[] ParseJson(string json)
    {
        JsonDocument document = JsonDocument.Parse(json);
        try
        {
            int length = document.RootElement.GetArrayLength();
            AdvertisementPhotoDao[] photos = new AdvertisementPhotoDao[length];
            int lastIndex = 0;

            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                string sourceUrl = element.GetProperty("Source").GetString()!;
                photos[lastIndex] = new AdvertisementPhotoDao() { SourceUrl = sourceUrl };
                lastIndex++;
            }

            document.Dispose();
            return photos;
        }
        catch
        {
            return [];
        }
        finally
        {
            document.Dispose();
        }
    }

    public IEnumerator<AdvertisementPhotoDao> GetEnumerator() => _photos.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _photos.Count;

    public AdvertisementPhotoDao this[int index] => _photos[index];
}
