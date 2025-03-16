using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Features.DeleteParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.DeleteParser;

[DependencyInjection]
public static class DeleteParsingRegistration
{
    [ServicesRegistration]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<DeleteParserCommand, Result>>(p =>
        {
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessager messager = factory.Create();
            DeleteParserContext context = new DeleteParserContext();
            DeleteParserCommandHandler h1 = new DeleteParserCommandHandler(context, messager);
            DeleteParserMapping h2 = new DeleteParserMapping(h1, context);
            DeleteParserLogging h3 = new DeleteParserLogging(h2, logger);
            return h3;
        });
    }
}
