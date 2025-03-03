using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Injections;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.CreateCustomTransportType;

public sealed record CreateCustomTransportTypeCommand(
    string Name,
    string Link,
    List<string>? Additions
) : IAvitoCommand
{
    public TransportType ToValueObject()
    {
        var createdOn = DateOnly.FromDateTime(DateTime.Now);
        return UserTransportType.Create(Name, Link, createdOn, Additions);
    }

    internal static void Register(IServiceCollection services)
    {
        services.AddScoped<CreateCustomTransportTypeCommandValidator>();
        services.AddScoped<
            IAvitoCommandHandler<CreateCustomTransportTypeCommand>,
            CreateCustomTransportTypeCommandHandler
        >();
    }
}

internal sealed class CreateCustomTransportTypeCommandHandler(
    ITransportTypesQueryRepository readRepository,
    ITransportTypesCommandRepository writeRepository,
    ILogger logger,
    CreateCustomTransportTypeCommandValidator validator
) : IAvitoCommandHandler<CreateCustomTransportTypeCommand>
{
    public async Task<Result> Handle(
        CreateCustomTransportTypeCommand command,
        CancellationToken ct = default
    )
    {
        var validation = await validator.ValidateAsync(command, ct);
        if (!validation.IsValid)
            return validation.LogAndReturnError(logger, nameof(CreateCustomTransportTypeCommand));

        var condition = new GetTransportTypeLinkCondition(command.Link);
        var response = await readRepository.Get(new GetTransportTypesQuery(Links: condition), ct);

        if (response.Items.Any())
        {
            var error = new Error($"Type with such link {command.Link} already exists.");
            return error.LogAndReturnError(logger, nameof(CreateCustomTransportTypeCommand));
        }

        var insertion = await writeRepository.Add(command.ToValueObject(), ct);
        return insertion.IsFailure ? insertion.Error : Result.Success();
    }
}
