using RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser;

[DependencyInjection]
public static class DeleteParsingRegistration
{
    [DependencyInjectionMethod]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<DeleteParserCommand, Result>>(p =>
        {
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessager messager = factory.Create();
            DeleteParserContext context = new();
            DeleteParserCommandHandler h1 = new(context, messager);
            DeleteParserMapping h2 = new(h1, context);
            DeleteParserLogging h3 = new(h2, logger);
            return h3;
        });
    }
}
