using RemTechCommon.Utils.OptionPattern;

namespace RemTech.MainApi.Common.Abstractions;

public interface IQuery<TResult> { }

public interface IQueryHandler<TQuery, TResult>
    where TQuery : IQuery<TResult>
{
    public Task<Option<TResult>> Handle(TQuery query, CancellationToken ct = default);
}
