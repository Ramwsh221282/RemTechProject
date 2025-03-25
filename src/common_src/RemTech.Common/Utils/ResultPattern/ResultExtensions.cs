namespace RemTechCommon.Utils.ResultPattern;

public sealed record ResultWhen<T>(bool Failed, Error? Error = null);

public sealed record WhenWithResult<T>(Result<T> Result);

public sealed record WhenWithResult(Result Result);

public static class ResultExtensions
{
    public static ResultWhen<T> When<T>(bool satisfied) => new(satisfied);

    public static ResultWhen<T> ApplyError<T>(this ResultWhen<T> when, string message) =>
        when.Failed ? when with { Error = new Error(message) } : when;

    public static ResultWhen<T> AlsoWhen<T>(this ResultWhen<T> when, bool satisfied) =>
        when.Failed ? when : When<T>(satisfied);

    public static Result<T> Process<T>(this ResultWhen<T> when, Func<Result<T>> func) =>
        when.Failed ? when.Error! : func();

    public static async Task<Result<T>> Process<T>(
        this ResultWhen<T> when,
        Func<Task<Result<T>>> func
    ) => when.Failed ? when.Error! : await func();

    public static WhenWithResult<T> ToWhen<T>(this Result<T> result) => new(result);

    public static WhenWithResult ToWhen(this Result result) => new WhenWithResult(result);

    public static WhenWithResult<T> IfFailure<T>(this WhenWithResult<T> when, Action action)
    {
        if (when.Result.IsFailure)
            action();
        return when;
    }

    public static WhenWithResult IfFailure(this WhenWithResult when, Action action)
    {
        if (when.Result.IsFailure)
            action();
        return when;
    }

    public static WhenWithResult<T> IfSuccess<T>(this WhenWithResult<T> when, Action action)
    {
        if (when.Result.IsSuccess)
            action();
        return when;
    }

    public static WhenWithResult IfSuccess(this WhenWithResult when, Action action)
    {
        if (when.Result.IsSuccess)
            action();
        return when;
    }

    public static Result<T> BackToResult<T>(this WhenWithResult<T> when) => when.Result;

    public static Result BackToResult(this WhenWithResult when) => when.Result;
}
