using Microsoft.Extensions.DependencyInjection;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Application.ParserContext.Features.UpdateParserProfile.Decorators;
using RemTech.Shared.SDK.DependencyInjection;
using Serilog;

namespace RemTech.Application.ParserContext.Features.UpdateParserProfile;

[InjectionClass]
public static class UpdateParserProfileCommandInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<UpdateParserProfileCommand, UnitResult<Guid>>>(p =>
        {
            IParserWriteRepository repository = p.GetRequiredService<IParserWriteRepository>();
            ILogger logger = p.GetRequiredService<ILogger>();
            IValidator<UpdateParserProfileCommand> validator =
                new UpdateParserProfileCommandValidator();

            UpdateParserProfileCommandHandler coreHandler = new(repository);
            UpdateParserProfileCommandValidating validating = new(coreHandler, validator);
            UpdateParserProfileCommandException exception = new(logger, validating);
            UpdateParserProfileCommandLogging logging = new(exception, logger);
            return logging;
        });
    }
}
