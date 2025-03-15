using RemTech.MainApi.ParsersManagement.Models;

namespace RemTech.MainApi.ParsersManagement.Dtos;

public sealed record ParserDto(
    string ParserName,
    string ParserState,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string[] Links
);

public static class ParserDtoExtensions
{
    public static ParserDto ToDto(this Parser parser) =>
        new ParserDto(
            parser.Name.Name,
            parser.State.State,
            parser.Schedule.RepeatEveryHours,
            parser.Schedule.LastRun,
            parser.Schedule.NextRun,
            parser.Links.Select(l => l.Url).ToArray()
        );
}
