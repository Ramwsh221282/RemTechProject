namespace SharedParsersLibrary.Models;

public static class ParserBehavior
{
    public static Parser SetWorkingState(this Parser parser) => parser with { State = "Работает" };

    public static Parser SetSleepingState(this Parser parser) => parser with { State = "Ожидание" };

    public static Parser UpdateSchedule(this Parser parser)
    {
        int repeatEvery = parser.RepeatEveryHours;
        DateTime lastRun = DateTime.Now;
        DateTime nextRun = lastRun.AddHours(repeatEvery);
        return parser with { LastRun = lastRun, NextRun = nextRun };
    }
}
