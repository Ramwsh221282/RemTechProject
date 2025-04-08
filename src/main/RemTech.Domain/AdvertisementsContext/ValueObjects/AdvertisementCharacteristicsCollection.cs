using System.Collections;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Domain.AdvertisementsContext.ValueObjects;

public sealed record AdvertisementCharacteristicsCollection
    : IReadOnlyList<AdvertisementCharacteristic>
{
    private static bool DuplicateCheck(
        AdvertisementCharacteristic arg1,
        AdvertisementCharacteristic arg2
    ) => arg1.Name == arg2.Name;

    public IReadOnlyList<AdvertisementCharacteristic> Characteristics { get; }

    private AdvertisementCharacteristicsCollection()
    {
        Characteristics = [];
    } // ef core

    private AdvertisementCharacteristicsCollection(AdvertisementCharacteristic[] characteristics) =>
        Characteristics = characteristics;

    public static Result<AdvertisementCharacteristicsCollection> Create(
        IEnumerable<AdvertisementCharacteristic> characteristics
    )
    {
        AdvertisementCharacteristic[] array = characteristics.ToArray();
        if (array.Length == 0)
            return new Error("Объявление без характеристик не допустимо.");

        if (!array.AreAllUnique(DuplicateCheck, out AdvertisementCharacteristic? duplicate))
        {
            string msg = $"Не уникальный список характеристик. Не уникальное - {duplicate!.Name}.";
            return new Error(msg);
        }

        return new AdvertisementCharacteristicsCollection(array);
    }

    public IEnumerator<AdvertisementCharacteristic> GetEnumerator() =>
        Characteristics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => Characteristics.Count;

    public AdvertisementCharacteristic this[int index] => Characteristics[index];
}
