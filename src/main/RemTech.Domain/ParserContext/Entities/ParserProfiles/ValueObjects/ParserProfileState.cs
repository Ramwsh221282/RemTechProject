using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

public sealed record ParserProfileState
{
    public static readonly ParserProfileState Sleeping = new("Простаивает");
    public static readonly ParserProfileState Working = new("Парсит");
    public static readonly ParserProfileState Disabled = new("Отключен");
    private static readonly ParserProfileState[] AllowedStates = [Sleeping, Working, Disabled];
    public string State { get; }

    private ParserProfileState(string state) => State = state;

    public static Result<ParserProfileState> Create(string state)
    {
        if (string.IsNullOrWhiteSpace(state))
            return ErrorFactory.EmptyOrNull(nameof(ParserProfileState));

        if (AllowedStates.All(s => s.State != state))
            return ErrorFactory.NotSupported(nameof(ParserProfileState), state);

        return new ParserProfileState(state);
    }
}
