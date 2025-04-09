using RemTech.Shared.SDK.Validators;

namespace RemTech.Shared.SDK.ResultPattern;

public sealed record UnitResult<T>(T? Result, string Message, int Code)
{
    private const string INTERNAL_SERVER_ERROR_MESSAGE =
        "Операция не была выполнена. Произошла внутренняя ошибка сервера";
    public bool IsFailure => !string.IsNullOrWhiteSpace(Message);
    public bool IsSuccess => string.IsNullOrWhiteSpace(Message);

    public static UnitResult<T> FromSuccess(T value) =>
        new(value, string.Empty, UnitResultCodes.Ok);

    public static implicit operator UnitResult<T>(T value) => FromSuccess(value);

    public static UnitResult<T> FromFailure(Error error, int statusCode) =>
        new(default, error.Description, statusCode);

    public static UnitResult<T> FromValidationFailure(
        ValidationResult validationResult,
        int statusCode
    )
    {
        if (validationResult.IsValid)
            throw new ArgumentException(
                "Нельзя создать Failure Unit Result из Validation Result Success"
            );
        return new UnitResult<T>(default, validationResult.ValidationMessage, statusCode);
    }

    public static UnitResult<T> InternalServerError() =>
        new(default, INTERNAL_SERVER_ERROR_MESSAGE, UnitResultCodes.InternalError);
}

public static class UnitResultExtensions
{
    public static UnitResult<T> FromValidationFailure<T>(this ValidationResult validationResult) =>
        UnitResult<T>.FromValidationFailure(validationResult, UnitResultCodes.BadRequest);

    public static UnitResult<T> FromResultFailure<T>(this Result result, int errorCode)
    {
        if (result.IsSuccess)
            throw new ArgumentException("Нельзя создать Failure Unit Result из Result Success");

        return UnitResult<T>.FromFailure(result.Error, errorCode);
    }
}
