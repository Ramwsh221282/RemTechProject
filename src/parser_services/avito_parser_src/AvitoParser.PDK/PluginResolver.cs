using System.Reflection;
using System.Runtime.Loader;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.PDK;

public sealed record PluginResolver
{
    private readonly ILogger _logger;

    public PluginResolver(ILogger logger) => _logger = logger;

    public Result<IAvitoPlugin> Resolve(
        AssemblyLoadContext context,
        string pluginPath,
        string pluginName
    )
    {
        Assembly assembly = context.LoadFromAssemblyPath(pluginPath);
        IEnumerable<Type> types = assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<PluginAttribute>() != null);

        foreach (Type type in types)
        {
            PluginAttribute attribute = type.GetCustomAttribute<PluginAttribute>()!;
            if (!attribute.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                continue;

            if (Activator.CreateInstance(type) is not IAvitoPlugin plugin)
                continue;
            _logger.Information(
                "{Context} resolved plugin {Name}",
                nameof(PluginResolver),
                pluginName
            );
            return Result<IAvitoPlugin>.Success(plugin);
        }

        _logger.Error("{Context} cannot resolve plugin {Name}", nameof(PluginResolver), pluginName);
        return new Error($"Cannot resolve plugin {pluginName}");
    }

    public Result<IAvitoPlugin<U>> Resolve<U>(
        AssemblyLoadContext context,
        string pluginPath,
        string pluginName
    )
    {
        Assembly assembly = context.LoadFromAssemblyPath(pluginPath);
        IEnumerable<Type> types = assembly
            .GetTypes()
            .Where(t => t.GetCustomAttribute<PluginAttribute>() != null);

        foreach (Type type in types)
        {
            PluginAttribute attribute = type.GetCustomAttribute<PluginAttribute>()!;
            if (!attribute.PluginName.Equals(pluginName, StringComparison.OrdinalIgnoreCase))
                continue;

            if (Activator.CreateInstance(type) is not IAvitoPlugin<U> plugin)
                continue;

            _logger.Information(
                "{Context} resolved plugin {Name}",
                nameof(PluginResolver),
                pluginName
            );
            return Result<IAvitoPlugin<U>>.Success(plugin);
        }

        _logger.Error("{Context} cannot resolve plugin {Name}", nameof(PluginResolver), pluginName);
        return new Error($"Cannot resolve plugin {pluginName}");
    }
}
