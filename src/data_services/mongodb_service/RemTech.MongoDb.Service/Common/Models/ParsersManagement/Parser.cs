using RemTech.MongoDb.Service.Features.ParserManagement.GetParser;

namespace RemTech.MongoDb.Service.Common.Models.ParsersManagement;

public sealed class Parser
{
    public string ServiceName { get; private init; }
    public string[] Links { get; private init; }
    public string State { get; private init; }
    public int RepeatEveryHours { get; private init; }
    public DateTime LastRun { get; private init; }
    public DateTime NextRun { get; private init; }

    public Parser(
        string serviceName,
        string[] links,
        string state,
        int repeatEveryHours,
        DateTime lastRun,
        DateTime nextRun
    ) =>
        (ServiceName, Links, State, RepeatEveryHours, LastRun, NextRun) = (
            serviceName,
            links,
            state,
            repeatEveryHours,
            lastRun,
            nextRun
        );
}

public static class ParserExtensions
{
    public static ParserDaoResponse ToParserDaoResponse(this Parser Parser) =>
        new ParserDaoResponse(
            Parser.ServiceName,
            Parser.RepeatEveryHours,
            Parser.LastRun,
            Parser.NextRun,
            Parser.State,
            Parser.Links.Select(l => l).ToArray()
        );
}
