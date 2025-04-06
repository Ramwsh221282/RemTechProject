using RemTech.Application.ParserContext.Dtos;
using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Validators;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile;

public sealed class UpdateParserProfileCommandValidator : IValidator<UpdateParserProfileCommand>
{
    public ValidationResult Validate(UpdateParserProfileCommand validatee)
    {
        UpdateParserProfileDto data = validatee.Data;

        if (data.Schedule is { RepeatEveryHours: not null })
        {
            Result<ParserProfileSchedule> schedule = ParserProfileSchedule.CreateFromHour(
                data.Schedule.RepeatEveryHours.Value
            );
            if (schedule.IsFailure)
                return ValidationResult.FromErrorResult(schedule);
        }

        if (data.ProfileState != null)
        {
            Result<ParserProfileState> state = ParserProfileState.Create(data.ProfileState);
            if (state.IsFailure)
                return ValidationResult.FromErrorResult(state);
        }

        if (data.Links != null)
        {
            Result<ParserProfileLink>[] results = [.. data.Links.Select(ParserProfileLink.Create)];
            Result<ParserProfileLink>? failure = results.FirstOrDefault(r => r.IsFailure);
            if (failure != null)
                return ValidationResult.FromErrorResult(failure);

            ParserProfileLink[] dataItems = [.. results.Select(r => r.Value)];
            Result<ParserProfileLinksCollection> collection = ParserProfileLinksCollection.Create(
                dataItems
            );
            if (collection.IsFailure)
                return ValidationResult.FromErrorResult(collection);
        }

        return ValidationResult.Success;
    }
}
