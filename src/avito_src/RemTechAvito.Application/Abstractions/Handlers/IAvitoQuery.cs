namespace RemTechAvito.Application.Abstractions.Handlers;

public interface IAvitoQuery<TResult>;

public interface IAvitoQueryHandler<in TQuery, TResult>
    where TQuery : class, IAvitoQuery<TResult>
{
    Task<TResult> Handle(TQuery query, CancellationToken ct = default);
}
