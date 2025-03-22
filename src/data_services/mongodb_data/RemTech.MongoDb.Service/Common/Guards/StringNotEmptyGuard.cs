using GuardValidationLibrary.GuardAbstractions;

namespace RemTech.MongoDb.Service.Common.Guards;

public sealed class StringNotEmptyGuard : ParameterGuard<string>
{
    public override ParameterGuardValidation Validate(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? new ParameterGuardValidationFailure("String argument was null or empty")
            : new ParameterGuardValidationSuccess();
}
