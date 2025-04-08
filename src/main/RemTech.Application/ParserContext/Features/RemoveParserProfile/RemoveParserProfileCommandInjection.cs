using Microsoft.Extensions.DependencyInjection;
using RemTech.Application.ParserContext.Contracts;
using RemTech.Application.ParserContext.Features.RemoveParserProfile.Decorators;
using RemTech.Shared.SDK.DependencyInjection;
using Serilog;

namespace RemTech.Application.ParserContext.Features.RemoveParserProfile;

[InjectionClass]
public static class RemoveParserProfileCommandInjection
{
    [InjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<RemoveParserProfileCommand, UnitResult<Guid>>>(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            IParserWriteRepository repository = p.GetRequiredService<IParserWriteRepository>();
            RemoveParserProfileCommandHandler coreHandler = new(repository);
            RemoveParserProfileLogging logging = new(coreHandler, logger);
            return logging;
        });
    }
}
