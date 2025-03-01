using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Responses.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement;
using RemTechAvito.Core.ParserProfileManagement.ValueObjects;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.ParserProfileManagement.CreateProfile;

public sealed record CreateProfileCommand(string Name) : IAvitoCommand<ParserProfileResponse>
{
    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<
            IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>,
            CreateProfileCommandHandler
        >();
        services.AddScoped<CreateProfileCommandValidator>();
    }

    public ParserProfileName GetName()
    {
        return ParserProfileName.Create(Name);
    }
}

internal sealed class CreateProfileCommandHandler(
    IParserProfileCommandRepository repository,
    ILogger logger,
    CreateProfileCommandValidator commandValidator
) : IAvitoCommandHandler<CreateProfileCommand, ParserProfileResponse>
{
    public async Task<Result<ParserProfileResponse>> Handle(
        CreateProfileCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await commandValidator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.LogAndReturnError(logger, nameof(CreateProfileCommand));

        var profile = new ParserProfile(command.GetName());
        var insertion = await repository.Add(profile, ct);
        if (insertion.IsFailure)
            return insertion.Error.LogAndReturnError(logger, nameof(CreateProfileCommand));

        return new ParserProfileResponse(
            profile.Id.Id,
            profile.CreatedOn.Date,
            profile.Name.Name,
            profile.State.IsActive,
            profile.State.Description,
            []
        );
    }
}
