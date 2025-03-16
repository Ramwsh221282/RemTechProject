using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Attributes;
using RemTech.MainApi.ParsersManagement.Features.UpdateParser.Decorators;
using RemTech.MainApi.ParsersManagement.Messages;
using RemTech.MainApi.ParsersManagement.Responses;
using ILogger = Serilog.ILogger;

namespace RemTech.MainApi.ParsersManagement.Features.UpdateParser;

[DependencyInjection]
public static class UpdateParserRegistration
{
    [ServicesRegistration]
    public static void Register(this IServiceCollection services)
    {
        services.AddScoped<ICommandHandler<UpdateParserCommand, ParserResponse>>(p =>
        {
            ILogger logger = p.GetRequiredService<ILogger>();
            DataServiceMessagerFactory factory = p.GetRequiredService<DataServiceMessagerFactory>();
            UpdateParserContext context = new();
            UpdateParserCommandHandler h1 = new(factory, context);
            UpdateParserValidator h2 = new(h1, context);
            UpdateParserExistanceChecker h3 = new(factory, h2);
            UpdateParserLogging h4 = new(logger, h3);
            return h4;
        });
    }
}
