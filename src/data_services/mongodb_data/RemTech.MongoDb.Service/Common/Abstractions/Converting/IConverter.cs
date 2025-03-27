using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MongoDb.Service.Common.Abstractions.Converting;

public interface IConverter<in TSource, TTarget>
{
    public Result<TTarget> Convert(TSource source);
}
