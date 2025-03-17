namespace RemTech.MainApi.ParsersManagement.Features.Shared;

public sealed record ParserQueryPayload(
    string? ServiceName = null,
    string[]? Links = null,
    string? State = null,
    int? RepeatEveryHours = null,
    DateTime? LastRun = null,
    DateTime? NextRun = null
);
