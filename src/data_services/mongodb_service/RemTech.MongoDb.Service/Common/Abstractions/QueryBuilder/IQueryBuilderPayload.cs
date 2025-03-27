using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;

public interface IQueryBuilderPayload<TModel>
{
    public bool IsPayloadEmpty { get; }
}

public interface IQueryBuilder<TPayload, TModel>
    where TPayload : IQueryBuilderPayload<TModel>
{
    public void SetPayload(TPayload payload);
    public FilterDefinition<TModel> Build();
}
