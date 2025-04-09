namespace RemTech.Shared.SDK.ResultPattern;

public sealed record Error(string Description)
{
    public static Error None = new(string.Empty);
}

public static class ErrorFactory
{
    public static Error EmptyOrNull(string property) => new($"{property} было пустым.");

    public static Error ExceesLength(string property, int length) =>
        new($"{property} превышает длину {length}.");

    public static Error NotSupported(string property, string value) =>
        new($"Значение {value} для {property} не поддерживается.");
}
