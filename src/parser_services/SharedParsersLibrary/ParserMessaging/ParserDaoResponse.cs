using SharedParsersLibrary.Models;

namespace SharedParsersLibrary.ParserMessaging;

internal sealed record ParserDaoResponse(
    string ParserName,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string State,
    string[] Links
);

internal static class ParserDaoResponseExtensions
{
    internal static Parser ToParser(this ParserDaoResponse response) =>
        new Parser(
            response.ParserName,
            response.Links,
            response.State,
            response.RepeatEveryHours,
            response.LastRun,
            response.NextRun
        );
}
