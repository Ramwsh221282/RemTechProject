using Microsoft.Extensions.DependencyInjection;
using SharedParsersLibrary.Configuration;
using SharedParsersLibrary.Sinking;

namespace SharedParsersLibrary.Attributes;

[AttributeUsage(AttributeTargets.Class)]
public sealed class ParserDependencyInjectionAttribute : Attribute { }

[AttributeUsage(AttributeTargets.Method)]
public sealed class ParserDependencyInjectionMethodAttribute : Attribute { }

[ParserDependencyInjection]
public static class ParserDependencyInjection
{
    [ParserDependencyInjectionMethod]
    public static void Inject(this IServiceCollection services)
    {
        services.AddSingleton(CreateConfiguration());
        services.AddTransient<SinkerSenderFactory>();
    }

    private static ScrapedAdvertisementsSinkerConfiguration CreateConfiguration()
    {
        ScrapedAdvertisementsSinkerConfigurationFactory factory = new();
        ScrapedAdvertisementsSinkerConfiguration conf = factory.Create();
        return conf;
    }
}
