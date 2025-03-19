using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.ParserMessaging;

internal sealed record ParserQueryPayload(
    string? ServiceName = null,
    string[]? Links = null,
    string? State = null,
    int? RepeatEveryHours = null,
    DateTime? LastRun = null,
    DateTime? NextRun = null
);

internal static class ParserQueryPayloadExtensions
{
    internal static ParserQueryPayload ToParserPayload(this Parser parser) =>
        new ParserQueryPayload(
            parser.ServiceName,
            parser.Links,
            parser.State,
            parser.RepeatEveryHours,
            parser.LastRun,
            parser.NextRun
        );
}
