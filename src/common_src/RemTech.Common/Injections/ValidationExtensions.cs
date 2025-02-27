using FluentValidation;
using FluentValidation.Results;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechCommon.Injections;

public static class ValidationExtensions
{
    public static IRuleBuilderOptions<T, TProperty> MustBeSuccessResult<T, TProperty, TValue>(
        this IRuleBuilder<T, TProperty> builder,
        Func<TProperty, Result<TValue>> factory
    )
    {
        return builder.Must(
            (_, property, context) =>
            {
                var result = factory(property);
                if (!result.IsFailure)
                    return true;
                context.AddFailure(result.Error.Description);
                return false;
            }
        );
    }

    public static Error LogAndReturnError(
        this ValidationResult validation,
        ILogger logger,
        string? context = null
    )
    {
        if (validation.IsValid)
            throw new ApplicationException("Cannot return error from success validation");
        var message = string.Join('\n', validation.Errors.Select(err => err.ErrorMessage));
        var error = new Error(message);
        if (context != null)
        {
            logger.Error("{Context}. {Message}", context, message);
            return error;
        }

        logger.Error("{Error}.", message);
        return error;
    }

    public static Error LogAndReturnError(this Error error, ILogger logger, string? context = null)
    {
        if (context != null)
        {
            logger.Error("{Context}. {Message}", context, error.Description);
            return error;
        }

        logger.Error("{Error}.", error.Description);
        return error;
    }
}
