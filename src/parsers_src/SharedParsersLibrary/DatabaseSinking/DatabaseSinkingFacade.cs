using RemTech.Domain.AdvertisementsContext;
using RemTech.Domain.AdvertisementsContext.ValueObjects;
using RemTech.Infrastructure.PostgreSql.Configuration;
using SharedParsersLibrary.DatabaseSinking.Queries;

namespace SharedParsersLibrary.DatabaseSinking;

public sealed class DatabaseSinkingFacade(ConnectionStringFactory factory)
{
    private readonly Dictionary<string, string> _characteristicsCache = [];
    private readonly HasAdvertisementQuery _hasAdvertisement = new(factory);
    private readonly InsertAdvertisementQuery _insertAdvertisement = new(factory);
    private readonly HasCharacteristicQuery _hasCharacteristic = new(factory);
    private readonly InsertCharacteristicQuery _insertCharacteristic = new(factory);

    public async Task<bool> HasAdvertisement(Advertisement advertisement) =>
        await _hasAdvertisement.Has(advertisement);

    public async Task InsertAdvertisement(Advertisement advertisement) =>
        await _insertAdvertisement.Insert(advertisement);

    public async Task<bool> HasCharacteristic(AdvertisementCharacteristic characteristic)
    {
        if (_characteristicsCache.ContainsKey(characteristic.Name))
            return true;

        bool has = await _hasCharacteristic.Has(characteristic);

        if (!has)
            _characteristicsCache.TryAdd(characteristic.Name, characteristic.Value);

        return has;
    }

    public async Task InsertCharacteristic(AdvertisementCharacteristic characteristic) =>
        await _insertCharacteristic.Insert(characteristic);
}
