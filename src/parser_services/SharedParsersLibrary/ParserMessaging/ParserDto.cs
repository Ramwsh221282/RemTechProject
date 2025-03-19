using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.ParserMessaging;

internal sealed record ParserDto(
    string ParserName,
    string ParserState,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string[] Links
);

internal static class ParserDtoExtensions
{
    internal static ParserDto ToParserDto(this Parser parser) =>
        new ParserDto(
            parser.ServiceName,
            parser.State,
            parser.RepeatEveryHours,
            parser.LastRun,
            parser.NextRun,
            parser.Links
        );
}
