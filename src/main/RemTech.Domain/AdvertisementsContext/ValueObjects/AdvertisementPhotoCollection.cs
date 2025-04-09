using System.Collections;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementPhotoCollection : IReadOnlyList<AdvertisementPhoto>
{
    private static bool DuplicateCheck(AdvertisementPhoto arg1, AdvertisementPhoto arg2) =>
        arg1 == arg2;

    public IReadOnlyList<AdvertisementPhoto> Photos { get; }

    private AdvertisementPhotoCollection()
    {
        Photos = [];
    } // ef core

    private AdvertisementPhotoCollection(AdvertisementPhoto[] photos) => Photos = photos;

    public static Result<AdvertisementPhotoCollection> Create(
        IEnumerable<AdvertisementPhoto> photos
    )
    {
        AdvertisementPhoto[] array = [.. photos];

        if (array.IsEmpty())
        {
            string message = "Фотографии список фотографий пуст.";
            return new Error(message);
        }

        if (!array.AreAllUnique(DuplicateCheck, out AdvertisementPhoto? _))
        {
            string message = "В объявлении есть дублирование ссылки на фотографию.";
            return new Error(message);
        }

        return new AdvertisementPhotoCollection(array);
    }

    public IEnumerator<AdvertisementPhoto> GetEnumerator() => Photos.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Photos.Count;

    public AdvertisementPhoto this[int index] => Photos[index];
}
