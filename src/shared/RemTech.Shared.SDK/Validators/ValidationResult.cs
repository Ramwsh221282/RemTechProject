using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Shared.SDK.Validators;

public sealed record ValidationResult(bool IsValid, string ValidationMessage)
{
    public static readonly ValidationResult Success = new(true, string.Empty);

    public static ValidationResult FromErrorResult(Result result)
    {
        if (result.IsSuccess)
            throw new ApplicationException(
                "Нельзя создать неправильный объект валидации при успешном результате."
            );

        return new ValidationResult(false, result.Error.Description);
    }
}
