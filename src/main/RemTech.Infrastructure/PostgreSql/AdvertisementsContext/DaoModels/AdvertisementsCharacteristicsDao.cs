using System.Collections;
using System.Text.Json;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;

public sealed class AdvertisementsCharacteristicsDao : IReadOnlyList<AdvertisementCharacteristicDao>
{
    private readonly List<AdvertisementCharacteristicDao> _characteristics;

    private AdvertisementsCharacteristicsDao(AdvertisementCharacteristicDao[] characteristics)
    {
        _characteristics = [.. characteristics];
    }

    public static AdvertisementsCharacteristicsDao Create(AdvertisementDao advertisement)
    {
        string json = advertisement.CharacteristicsJson;

        if (string.IsNullOrWhiteSpace(json))
            return new AdvertisementsCharacteristicsDao([]);

        AdvertisementCharacteristicDao[] characteristics = ParseJson(json);
        return new AdvertisementsCharacteristicsDao(characteristics);
    }

    private static AdvertisementCharacteristicDao[] ParseJson(string json)
    {
        JsonDocument document = JsonDocument.Parse(json);

        try
        {
            int length = document.RootElement.GetArrayLength();
            AdvertisementCharacteristicDao[] array = new AdvertisementCharacteristicDao[length];
            int lastIndex = 0;
            foreach (JsonElement element in document.RootElement.EnumerateArray())
            {
                string name = element.GetProperty("Name").GetString()!;
                string value = element.GetProperty("Value").GetString()!;

                array[lastIndex] = new AdvertisementCharacteristicDao()
                {
                    Name = name,
                    Value = value,
                };

                lastIndex++;
            }

            document.Dispose();
            return array;
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

    public IEnumerator<AdvertisementCharacteristicDao> GetEnumerator() =>
        _characteristics.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _characteristics.Count;

    public AdvertisementCharacteristicDao this[int index] => _characteristics[index];
}
