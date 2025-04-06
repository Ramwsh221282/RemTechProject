using Microsoft.Extensions.DependencyInjection;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Application.ParserContext.Features.AddParserProfile.Decorators;
using RemTech.Shared.SDK.CqrsPattern.Commands;
using RemTech.Shared.SDK.DependencyInjection;
using RemTech.Shared.SDK.ResultPattern;
using RemTech.Shared.SDK.Validators;
using Serilog;

namespace RemTech.Application.ParserContext.Features.AddParserProfile;

[InjectionClass]
public static class AddParserProfileCommandInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<AddParserProfileCommand, UnitResult<Guid>>>(p =>
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
