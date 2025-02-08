using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Injections;

public static class PluginLoaderExtensions
{
    public static IServiceCollection LoadPlugin(this IServiceCollection services, string pluginPath)
    {
        if (string.IsNullOrWhiteSpace(pluginPath))
            throw new FileNotFoundException("No plugin path provided");

        if (!Directory.Exists(pluginPath))
            throw new FileNotFoundException(
                $"The plugin path provided does not exist: {pluginPath}"
            );

        string[] files = Directory.GetFiles(pluginPath, "*.dll");
        if (files.Length == 0)
            throw new FileNotFoundException("Plugin files not found");

        Assembly[] assemblies = files.Select(Assembly.LoadFile).ToArray();
        services.Scan(x =>
            x.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IPluginLoader>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

        using IServiceScope scope = services.BuildServiceProvider().CreateScope();
        IPluginLoader loader = scope.ServiceProvider.GetRequiredService<IPluginLoader>();
        services = loader.RegisterServices(services);
        return services;
    }
}
