using System.Collections;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportTypes;

public sealed class TransportTypesCollection : IReadOnlyCollection<TransportType>
{
    private readonly List<TransportType> _types = [];

    public const string CollectionName = "Тип техники";

    public DateOnly CreatedOn { get; } = DateOnly.FromDateTime(DateTime.Now);

    public Guid Id { get; } = Guid.NewGuid();

    public Result Add(TransportType type)
    {
        if (_types.Any(t => t == type))
            return new Error($"Transport type with name {type} already exists");

        _types.Add(type);
        return Result.Success();
    }

    public IEnumerator<TransportType> GetEnumerator() => _types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _types.Count;

    public Result<TransportType> this[int index] => _types.GetByIndex(index);
}
