using System.Collections;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerTypes;

public sealed class CustomerTypesCollection : IReadOnlyCollection<CustomerType>
{
    private readonly List<CustomerType> _types = [];
    public const string CollectionName = "Типы продавцов";
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly DateCreated { get; } = DateOnly.FromDateTime(DateTime.Now);

    public Result Add(CustomerType type)
    {
        if (_types.Any(t => t == type))
            return new Error("Such type already exists");
        _types.Add(type);
        return Result.Success();
    }

    public IEnumerator<CustomerType> GetEnumerator() => _types.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _types.Count;

    public Result<CustomerType> this[int index] => _types.GetByIndex(index);
}
