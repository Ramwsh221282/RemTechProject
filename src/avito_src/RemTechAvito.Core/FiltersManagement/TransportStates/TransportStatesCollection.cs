using System.Collections;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportStates;

public sealed class TransportStatesCollection : IReadOnlyCollection<TransportState>
{
    private readonly List<TransportState> _states = [];
    public const string CollectionName = "Состояние";
    public Guid Id { get; } = Guid.NewGuid();
    public DateOnly DateCreated { get; } = DateOnly.FromDateTime(DateTime.Now);

    public Result Add(TransportState state)
    {
        if (_states.Any(s => s == state))
            return new Error("Such state already exists");

        _states.Add(state);
        return Result.Success();
    }

    public IEnumerator<TransportState> GetEnumerator() => _states.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

    public int Count => _states.Count;

    public Result<TransportState> this[int index] => _states.GetByIndex(index);
}
