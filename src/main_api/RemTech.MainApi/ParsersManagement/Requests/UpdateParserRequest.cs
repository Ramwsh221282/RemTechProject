namespace RemTech.MainApi.ParsersManagement.Requests;

public record UpdateParserRequest(
    string ParserState,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun,
    string[] Links
);
