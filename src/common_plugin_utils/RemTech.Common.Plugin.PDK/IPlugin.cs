using RemTechCommon.Utils.ResultPattern;

namespace RemTech.Common.Plugin.PDK;

public interface IPlugin
{
    Task<Result> Execute(PluginPayload? payload);
}

public interface IPlugin<TResult>
{
    Task<Result<TResult>> Execute(PluginPayload? payload);
}
