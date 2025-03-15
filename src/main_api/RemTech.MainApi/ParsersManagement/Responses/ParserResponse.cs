using RemTech.MainApi.ParsersManagement.Models;

namespace RemTech.MainApi.ParsersManagement.Responses;

public sealed record ParserResponse(
    string ParserName,
    string ParserState,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string[] Links
);

public static class ParserResponseExtensions
{
    public static ParserResponse ToResponse(this Parser parser) =>
        new ParserResponse(
            parser.Name.Name,
            parser.State.State,
            parser.Schedule.RepeatEveryHours,
            parser.Schedule.LastRun,
            parser.Schedule.NextRun,
            parser.Links.Select(l => l.Url).ToArray()
        );
}
