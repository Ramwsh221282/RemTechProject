namespace RemTech.MongoDb.Service.Common.Models.ParsersManagement;

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
    public static Parser MapToParserModel(this ParserDto dto) =>
        new Parser(
            dto.ParserName,
            dto.Links,
            dto.ParserState,
            dto.RepeatEveryHours,
            dto.LastRun,
            dto.NextRun
        );
}
