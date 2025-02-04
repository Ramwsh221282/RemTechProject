using System.Reflection;
using System.Runtime.Loader;
using Microsoft.Extensions.DependencyInjection;

namespace RemTechCommon.Injections;

public static class PluginInjector
{
    public static IServiceCollection Inject(this IServiceCollection services, string path)
    {
        ReadOnlySpan<string> dllFiles = Directory.GetFiles(path, "*.dll");
        if (dllFiles.Length == 0)
            throw new FileNotFoundException("No plugins found in the specified directory.", path);

        AssemblyLoadContext context = new AssemblyLoadContext(path, true);

        Assembly[] assemblies = Directory
            .GetFiles(path, "*.dll")
            .Select(Assembly.LoadFrom)
            .ToArray();

        Array.ForEach(assemblies, assembly => context.LoadFromAssemblyPath(assembly.Location));

        services.Scan(scanning =>
            scanning
                .FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo<IPlugin>())
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

        IServiceProvider provider = services.BuildServiceProvider();

        using IServiceScope scope = provider.CreateScope();
        IEnumerable<IPlugin> plugins = scope.ServiceProvider.GetServices<IPlugin>();
        foreach (IPlugin plugin in plugins)
            plugin.Inject(services);

        context.Unload();

        GC.Collect();
        GC.WaitForPendingFinalizers();

        return services;
    }
}
