namespace RemTechCommon;

public sealed record Error(string Description)
{
    public static Error None = new("");
}

public sealed class Result<T>
{
    public T Value { get; } = default!;
    public Error Error { get; }

    public bool IsSuccess;
    public bool IsFailure;

    private Result(T value)
    {
        Value = value;
        IsSuccess = true;
        Error = Error.None;
    }

    private Result(Error error)
    {
        Error = error;
        IsFailure = true;
    }

    public static Result<T> Success(T value) => new(value);

    public static Result<T> Failure(Error error) => new(error);

    public static implicit operator T(Result<T> result) => result.Value;

    public static implicit operator Result<T>(T value) => Success(value);

    public static implicit operator Result<T>(Error error) => Failure(error);
}
