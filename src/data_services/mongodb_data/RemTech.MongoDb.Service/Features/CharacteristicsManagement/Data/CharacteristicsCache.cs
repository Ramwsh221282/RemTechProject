using System.Collections.Concurrent;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Data;

public sealed class CharacteristicsCache
{
    private readonly ConcurrentDictionary<string, Guid> _cache = [];

    public CharacteristicsCache(Characteristic[] characteristics)
    {
        _cache = [];
        foreach (var characteristic in characteristics)
        {
            if (!_cache.ContainsKey(characteristic.Name))
            {
                _cache.TryAdd(characteristic.Name, characteristic.Id);
            }
        }
        Console.WriteLine("Cache initialized");
    }

    public void Add(Characteristic characteristic) =>
        _cache.TryAdd(characteristic.Name, characteristic.Id);

    public bool Contains(Characteristic characteristic) => _cache.ContainsKey(characteristic.Name);

    public bool IsEmpty => _cache.IsEmpty;

    public Characteristic[] Get()
    {
        Characteristic[] ctx = new Characteristic[_cache.Count];
        int lastIndex = 0;
        foreach (var item in _cache)
        {
            ctx[lastIndex] = new Characteristic(item.Key);
            lastIndex++;
        }
        return ctx;
    }
}
