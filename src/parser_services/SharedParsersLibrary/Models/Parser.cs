namespace SharedParsersLibrary.Models;

public sealed record Parser(
    string ServiceName,
    string[] Links,
    string State,
    int RepeatEveryHours,
    DateTime LastRun,
    DateTime NextRun
);
