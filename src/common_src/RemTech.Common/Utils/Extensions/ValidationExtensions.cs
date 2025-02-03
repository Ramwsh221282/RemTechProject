using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.Extensions;

public static class ValidationExtensions
{
    public static Error ToError(this ValidationResult result)
    {
        ValidationFailure failure = result.Errors.Single();
        string message = failure.ErrorMessage;
        return new Error(message);
    }
}
