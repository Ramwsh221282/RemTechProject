using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK;

public interface IAvitoPlugin
{
    Task<Result> Execute(AvitoPluginPayload? payload);
}

public interface IAvitoPlugin<TResult>
{
    Task<Result<TResult>> Execute(AvitoPluginPayload? payload);
}
