using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Common.Abstractions.Converting;

public static class ConvertExecutor
{
    public static Result<TTarget> ExecuteConvert<TSource, TTarget>(
        this IConverter<TSource, TTarget> converter,
        TSource source
    ) => converter.Convert(source);
}
