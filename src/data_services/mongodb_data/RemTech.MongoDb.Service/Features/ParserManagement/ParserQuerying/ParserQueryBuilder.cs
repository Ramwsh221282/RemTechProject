using MongoDB.Bson;
using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;

namespace RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;

public sealed class ParserQueryBuilder
    : BaseQueryBuilder<Parser>,
        IQueryBuilder<ParserQueryPayload, Parser>
{
    private ParserQueryPayload? _payload;

    public void SetPayload(ParserQueryPayload payload) => _payload = payload;

    public FilterDefinition<Parser> Build()
    {
        if (_payload == null)
            return Empty;

        if (_payload.IsPayloadEmpty)
            return Empty;

        ApplyNameFilter();
        ApplyLinksFilter();
        ApplyStateFilter();
        ApplyRepeatEveryHoursFilter();
        ApplyLastRunTime();
        ApplyNextRunTime();
        FilterDefinition<Parser> combined = Combined;
        return combined;
    }

    private void ApplyNameFilter()
    {
        string? name = _payload!.ServiceName;
        if (string.IsNullOrWhiteSpace(name))
            return;
        With(new BsonDocument("ServiceName", new BsonDocument("$eq", name)));
    }

    private void ApplyLinksFilter()
    {
        string[]? links = _payload!.Links;
        if (links == null)
            return;
        if (links.Length == 0)
            return;
        With(new BsonDocument("ServiceName", new BsonDocument("$in", new BsonArray(links))));
    }

    private void ApplyStateFilter()
    {
        string? state = _payload!.State;
        if (string.IsNullOrWhiteSpace(state))
            return;
        With(new BsonDocument("State", new BsonDocument("$eq", state)));
    }

    private void ApplyRepeatEveryHoursFilter()
    {
        int? hours = _payload!.RepeatEveryHours;
        if (hours == null)
            return;
        With(new BsonDocument("RepeatEveryHours", new BsonDocument("$eq", hours)));
    }

    private void ApplyLastRunTime()
    {
        DateTime? lastRun = _payload!.LastRun;
        if (lastRun == null)
            return;
        With(new BsonDocument("LastRun", new BsonDocument("$eq", lastRun)));
    }

    private void ApplyNextRunTime()
    {
        DateTime? nextRun = _payload!.NextRun;
        if (nextRun == null)
            return;
        With(new BsonDocument("NextRun", new BsonDocument("$eq", nextRun)));
    }
}
