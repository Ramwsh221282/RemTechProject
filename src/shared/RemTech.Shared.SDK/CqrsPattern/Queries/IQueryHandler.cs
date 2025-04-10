namespace RemTech.Shared.SDK.CqrsPattern.Queries;

public interface IQueryHandler<in TQuery, TQueryResult>
    where TQuery : IQuery
{
    Task<TQueryResult> Handle(TQuery query, CancellationToken ct = default);
}
