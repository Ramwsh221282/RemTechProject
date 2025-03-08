using System.Reflection;
using System.Runtime.Loader;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Common.Plugin.PDK;

public sealed record PluginResolver
{
    private readonly ILogger _logger;

    public PluginResolver(ILogger logger) => _logger = logger;

    public Result<IPlugin> Resolve(
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

            if (Activator.CreateInstance(type) is not IPlugin plugin)
                continue;
            _logger.Information(
                "{Context} resolved plugin {Name}",
                nameof(PluginResolver),
                pluginName
            );
            return Result<IPlugin>.Success(plugin);
        }

        _logger.Error("{Context} cannot resolve plugin {Name}", nameof(PluginResolver), pluginName);
        return new Error($"Cannot resolve plugin {pluginName}");
    }

    public Result<IPlugin<U>> Resolve<U>(
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

            if (Activator.CreateInstance(type) is not IPlugin<U> plugin)
                continue;

            _logger.Information(
                "{Context} resolved plugin {Name}",
                nameof(PluginResolver),
                pluginName
            );
            return Result<IPlugin<U>>.Success(plugin);
        }

        _logger.Error("{Context} cannot resolve plugin {Name}", nameof(PluginResolver), pluginName);
        return new Error($"Cannot resolve plugin {pluginName}");
    }
}
