using RemTech.MainApi.ParsersManagement.Features.CreateParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Models;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.CreateParser;

[DependencyInjection]
public static class CreateParserCommandRegistration
{
    [DependencyInjectionMethod]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<CreateParserCommand, Result<Parser>>>(p =>
        {
            DataServiceMessagerFactory f = p.GetRequiredService<DataServiceMessagerFactory>();
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessager messager = f.Create();
            CreateParserContext context = new();
            CreateParserHandler h1 = new(messager, context);
            CreateParserValidating h2 = new(h1, context);
            CreateParserLogging h3 = new(h2, logger);
            return h3;
        });
    }
}
