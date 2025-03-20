using GuardValidationLibrary.Attributes;

namespace RemTech.MainApi.ParsersManagement.Models;

public sealed record Parser
{
    public ParserName Name { get; private init; }
    public ParserSchedule Schedule { get; private init; }
    public ParserState State { get; private init; }
    public ParserLink[] Links { get; private init; }

    [GuardedConstructor]
    private Parser(
        [GuardedParameter(typeof(ParserNameGuard))] ParserName name,
        [GuardedParameter(typeof(ParserScheduleGuard))] ParserSchedule schedule,
        [GuardedParameter(typeof(ParserStateGuard))] ParserState state,
        [GuardedParameter(typeof(ParserLinksGuard))] ParserLink[] links
    ) => (Name, Schedule, State, Links) = (name, schedule, state, links);

    [GuardedConstructor]
    private Parser([GuardedParameter(typeof(ParserNameGuard))] ParserName name) =>
        (Name, Schedule, State, Links) = (
            name,
            new ParserSchedule(1, DateTime.Now, DateTime.Now.AddHours(1)),
            new ParserState("Отключен"),
            Array.Empty<ParserLink>()
        );
}

public sealed record ParserSchedule(int RepeatEveryHours, DateTime LastRun, DateTime NextRun);

public sealed record ParserState(string State)
{
    public static string[] AllowedStates => ["Ожидание", "Работает", "Отключен"];
}

public sealed record ParserLink(string Url);

public sealed record ParserName(string Name);
