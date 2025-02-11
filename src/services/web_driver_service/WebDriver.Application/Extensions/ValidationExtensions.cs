using FluentValidation.Results;
using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace WebDriver.Application.Extensions;

internal static class ValidationExtensions
{
    private static string ExceptionMessage = "Cannot return valid result as error";

    internal static Error LogAndReturn(this ValidationResult validation, ILogger logger)
    {
        if (validation.IsValid)
            throw new ApplicationException(ExceptionMessage);
        Error error = validation.ToError();
        logger.Error("{Error}", error.Description);
        return error;
    }

    internal static Error LogAndReturn(this Result result, ILogger logger)
    {
        if (result.IsSuccess)
            throw new ApplicationException(ExceptionMessage);
        Error error = result.Error;
        logger.Error("{Error}", error.Description);
        return error;
    }
}
