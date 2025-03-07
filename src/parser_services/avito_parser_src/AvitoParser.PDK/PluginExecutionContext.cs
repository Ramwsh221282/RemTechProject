using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace AvitoParser.PDK;

public class PluginExecutionContext
{
    private readonly ILogger _logger;
    private readonly PluginFileValidator _pathValidator;
    private readonly PluginResolver _resolver;

    public PluginExecutionContext(
        ILogger logger,
        PluginFileValidator pathValidator,
        PluginResolver resolver
    ) => (_logger, _pathValidator, _resolver) = (logger, pathValidator, resolver);

    public async Task<Result> ExecutePlugin(
        string pluginPath,
        string pluginName,
        AvitoPluginPayload? payload = null
    )
    {
        (WeakReference, Result) execution = await Execute(pluginPath, pluginName, payload);
        while (execution.Item1.IsAlive)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return execution.Item2;
    }

    public async Task<Result<U>> ExecutePlugin<U>(
        string pluginPath,
        string pluginName,
        AvitoPluginPayload? payload = null
    )
    {
        (WeakReference, Result<U>) execution = await Execute<U>(pluginPath, pluginName, payload);
        while (execution.Item1.IsAlive)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return execution.Item2;
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private async Task<(WeakReference, Result)> Execute(
        string pluginPath,
        string pluginName,
        AvitoPluginPayload? payload = null
    )
    {
        AssemblyLoadContext loadContext = new AssemblyLoadContext(pluginPath, isCollectible: true);
        try
        {
            Result pathValidation = _pathValidator.Validate(pluginPath);
            if (pathValidation.IsFailure)
                return (new WeakReference(loadContext), pathValidation.Error);

            Result<IAvitoPlugin> resolve = _resolver.Resolve(loadContext, pluginPath, pluginName);
            if (resolve.IsFailure)
                return (new WeakReference(loadContext), resolve.Error);

            IAvitoPlugin plugin = resolve.Value;
            Result result = await plugin.Execute(payload);
            return (new WeakReference(loadContext), result);
        }
        catch (Exception ex)
        {
            _logger.Fatal("Exception at executing {Plugin}. {Ex}", pluginName, ex.Message);
            return (
                new WeakReference(loadContext),
                new Error($"Cannot execute plugin with name: {pluginName}")
            );
        }
        finally
        {
            loadContext.Unload();
        }
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private async Task<(WeakReference, Result<U>)> Execute<U>(
        string pluginPath,
        string pluginName,
        AvitoPluginPayload? payload = null
    )
    {
        AssemblyLoadContext loadContext = new AssemblyLoadContext(pluginPath, isCollectible: true);
        try
        {
            Result pathValidation = _pathValidator.Validate(pluginPath);
            if (pathValidation.IsFailure)
                return (new WeakReference(loadContext), pathValidation.Error);

            Result<IAvitoPlugin<U>> resolve = _resolver.Resolve<U>(
                loadContext,
                pluginPath,
                pluginName
            );
            if (resolve.IsFailure)
                return (new WeakReference(loadContext), resolve.Error);

            IAvitoPlugin<U> plugin = resolve.Value;
            Result<U> result = await plugin.Execute(payload);
            return (new WeakReference(loadContext), result);
        }
        catch (Exception ex)
        {
            _logger.Fatal("Exception at executing {Plugin}. {Ex}", pluginName, ex.Message);
            return (
                new WeakReference(loadContext),
                new Error($"Cannot execute plugin with name: {pluginName}")
            );
        }
        finally
        {
            loadContext.Unload();
        }
    }
}
