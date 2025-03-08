using Microsoft.Extensions.DependencyInjection;

namespace RemTech.Common.Plugin.PDK;

public static class PluginPDKInjection
{
    public static void InjectPluginPDK(this IServiceCollection services)
    {
        services.AddTransient<PluginExecutionContext>();
        services.AddTransient<PluginFileValidator>();
        services.AddTransient<PluginResolver>();
    }
}
