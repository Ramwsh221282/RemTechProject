using RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using RemTechCommon.Utils.DependencyInjectionHelpers;
using RemTechCommon.Utils.ResultPattern;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser;

[DependencyInjection]
public static class UpdateParserRegistration
{
    [DependencyInjectionMethod]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<IRequestHandler<UpdateParserCommand, Result<ParserResponse>>>(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            UpdateParserContext context = new();
            UpdateParserCommandHandler h1 = new(factory, context);
            UpdateParserValidator h2 = new(h1, context);
            UpdateParserExistanceChecker h3 = new(factory, h2);
            UpdateParserLogging h4 = new(h3, logger);
            return h4;
        });
    }
}
