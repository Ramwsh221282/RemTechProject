using System.Runtime.CompilerServices;
using System.Runtime.Loader;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTech.Common.Plugin.PDK;

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

    public async Task<Result> ExecutePlugin(PluginCommand command)
    {
        (WeakReference, Result) execution = await Execute(
            command.PluginPath,
            command.PluginName,
            command.Payload
        );

        for (int index = 0; index < 10 && execution.Item1.IsAlive; index++)
        {
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        return execution.Item2;
    }

    public async Task<Result<U>> ExecutePlugin<U>(PluginCommand command)
    {
        (WeakReference, Result<U>) execution = await Execute<U>(
            command.PluginPath,
            command.PluginName,
            command.Payload
        );

        for (int index = 0; index < 10 && execution.Item1.IsAlive; index++)
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
        PluginPayload? payload = null
    )
    {
        AssemblyLoadContext loadContext = new AssemblyLoadContext(pluginPath, isCollectible: true);
        try
        {
            Result pathValidation = _pathValidator.Validate(pluginPath);
            if (pathValidation.IsFailure)
                return (new WeakReference(loadContext), pathValidation.Error);

            Result<IPlugin> resolve = _resolver.Resolve(loadContext, pluginPath, pluginName);
            if (resolve.IsFailure)
                return (new WeakReference(loadContext), resolve.Error);

            IPlugin plugin = resolve.Value;
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
        PluginPayload? payload = null
    )
    {
        AssemblyLoadContext loadContext = new AssemblyLoadContext(pluginPath, isCollectible: true);
        try
        {
            Result pathValidation = _pathValidator.Validate(pluginPath);
            if (pathValidation.IsFailure)
                return (new WeakReference(loadContext), pathValidation.Error);

            Result<IPlugin<U>> resolve = _resolver.Resolve<U>(loadContext, pluginPath, pluginName);
            if (resolve.IsFailure)
                return (new WeakReference(loadContext), resolve.Error);

            IPlugin<U> plugin = resolve.Value;
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

public static class PluginExecutionContextExtensions
{
    public static async Task<Result<U>> Execute<U>(
        this PluginExecutionContext context,
        string pluginName,
        PluginPayload? payload = null
    )
    {
        PluginCommand command = new PluginCommand(pluginName, payload);
        Result<U> result = await context.ExecutePlugin<U>(command);
        return result;
    }

    public static async Task<Result> Execute(
        this PluginExecutionContext context,
        string pluginName,
        PluginPayload? payload = null
    )
    {
        PluginCommand command = new PluginCommand(pluginName, payload);
        Result result = await context.ExecutePlugin(command);
        return result;
    }
}
