using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Models;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

[DependencyInjection]
public static class CreateParserCommandRegistration
{
    [ServicesRegistration]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<CreateParserCommand, Parser>>(p =>
        {
            DataServiceMessagerFactory f = p.GetRequiredService<DataServiceMessagerFactory>();
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessager messager = f.Create();
            CreateParserContext context = new CreateParserContext();
            CreateParserHandler h1 = new CreateParserHandler(messager, context);
            CreateParserValidating h2 = new CreateParserValidating(context, h1);
            CreateParserLogging h3 = new CreateParserLogging(h2, logger);
            return h3;
        });
    }
}
