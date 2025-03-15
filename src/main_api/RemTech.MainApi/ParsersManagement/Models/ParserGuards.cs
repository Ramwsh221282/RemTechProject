using GuardValidationLibrary.GuardAbstractions;
using RemTechCommon.Utils.Extensions;

namespace RemTech.MainApi.ParsersManagement.Models;

public sealed class ParserNameGuard : ParameterGuard<ParserName>
{
    public override ParameterGuardValidation Validate(ParserName value)
    {
        string name = value.Name;
        return string.IsNullOrEmpty(name)
            ? new ParameterGuardValidationFailure("Parser name is required.")
            : new ParameterGuardValidationSuccess();
    }
}

public sealed class ParserStateGuard : ParameterGuard<ParserState>
{
    public override ParameterGuardValidation Validate(ParserState value)
    {
        string[] allowedStates = ParserState.AllowedStates;
        string state = value.State;
        return allowedStates.Contains(state) ? new ParameterGuardValidationSuccess()
            : string.IsNullOrWhiteSpace(state)
                ? new ParameterGuardValidationFailure("Parser state is required.")
            : new ParameterGuardValidationFailure($"Parser state {state} is not supported.");
    }
}

public sealed class ParserLinksGuard : ParameterGuard<ParserLink[]>
{
    public override ParameterGuardValidation Validate(ParserLink[] value)
    {
        Dictionary<string, int> _cache = [];

        foreach (var item in value)
        {
            if (_cache.ContainsKey(item.Url))
                return new ParameterGuardValidationFailure(
                    "Parser links duplicates are not allowed."
                );

            if (string.IsNullOrWhiteSpace(item.Url))
                return new ParameterGuardValidationFailure("None of parser links shall be empty.");

            if (!UrlLinkValidator.IsStringUrl(item.Url))
                return new ParameterGuardValidationFailure($"Parser link {item.Url} is invalid.");

            _cache.Add(item.Url, 1);
        }

        return new ParameterGuardValidationSuccess();
    }
}

public sealed class ParserScheduleGuard : ParameterGuard<ParserSchedule>
{
    public override ParameterGuardValidation Validate(ParserSchedule value)
    {
        int hours = value.RepeatEveryHours;
        return (hours <= 0)
            ? new ParameterGuardValidationFailure("Parser schedule hours is invalid")
            : new ParameterGuardValidationSuccess();
    }
}
