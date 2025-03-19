using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Attributes;

namespace SharedParsersLibrary.ParserMessaging;

[ParserDependencyInjection]
public static class ParserMessagingDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddTransient<ParserMessageSender>();
    }
}
