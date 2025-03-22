using RemTech.MongoDb.Service.Common.Abstractions.QueryBuilder;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;

namespace RemTech.MongoDb.Service.Features.ParserManagement.ParserQuerying;

public sealed record ParserQueryPayload(
    string? ServiceName = null,
    string[]? Links = null,
    string? State = null,
    int? RepeatEveryHours = null,
    DateTime? LastRun = null,
    DateTime? NextRun = null
) : IQueryBuilderPayload<Parser>
{
    public bool IsPayloadEmpty =>
        ServiceName == null
        && Links == null
        && State == null
        && RepeatEveryHours == null
        && LastRun == null
        && NextRun == null;
}
