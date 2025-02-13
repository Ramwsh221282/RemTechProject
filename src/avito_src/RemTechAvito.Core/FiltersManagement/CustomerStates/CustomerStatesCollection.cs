using System.Collections;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.CustomerStates;

public sealed class CustomerStatesCollection : IReadOnlyCollection<CustomerState>
{
    public const string CollectionName = "Статус продавца";
    private readonly List<CustomerState> _states = [];
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly CreatedOn { get; } = DateOnly.FromDateTime(DateTime.Now);

    public Result Add(CustomerState state)
    {
        if (_states.Any(s => s == state))
            return new Error("Such state already exists");

        _states.Add(state);
        return Result.Success();
    }

    public IEnumerator<CustomerState> GetEnumerator() => _states.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _states.Count;

    public Result<CustomerState> this[int index] => _states.GetByIndex(index);
}
