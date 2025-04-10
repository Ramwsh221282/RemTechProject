using Microsoft.Extensions.DependencyInjection;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;
using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Shared.SDK.DependencyInjection;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile;

[InjectionClass]
public static class AddParserProfileCommandInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddParserProfileCommand, UnitResult<ParserProfile>>>(p =>
        {
            IParserWriteRepository repository = p.GetRequiredService<IParserWriteRepository>();
            ILogger logger = p.GetRequiredService<ILogger>();
            IValidator<AddParserProfileCommand> validator = new AddParserProfileCommandValidator();

            AddParserProfileCommandHandler coreHandler = new(repository);
            AddParserProfileValidating validating = new(validator, coreHandler);
            AddParserProfileExceptionSupressing exception = new(validating, logger);
            AddParserProfileLogging logging = new AddParserProfileLogging(logger, exception);
            return logging;
        });
    }
}
