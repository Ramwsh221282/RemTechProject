using RemTechCommon.Utils.ResultPattern;

namespace AvitoParser.PDK;

public sealed record AvitoPluginPayload(params object[]? Data);

public sealed record AvitoPluginPayloadResolver(AvitoPluginPayload? Payload)
{
    public Result<U> Resolve<U>()
    {
        if (Payload?.Data == null)
            return new Error("Payload is empty or null");

        Type requestedType = typeof(U);
        object? element = Payload.Data.FirstOrDefault(obj =>
            obj.GetType().IsAssignableTo(requestedType)
        );

        return element switch
        {
            null => new Error($"Cannot resolve element of data type: {requestedType.Name}"),
            U obj => obj,
            _ => new Error("Cannot resolve element of data type: " + element.GetType().Name),
        };
    }

    public List<U> ResolveMany<U>()
    {
        if (Payload?.Data == null)
            return [];

        Type requestedType = typeof(U);
        List<object> elements = Payload
            .Data.Where(obj => obj.GetType().IsAssignableTo(requestedType))
            .ToList();

        return elements.Cast<U>().ToList();
    }

    public Result<U> ResolveByCompare<U>(Func<U, bool> predicate)
    {
        Result<U> existed = Resolve<U>();
        if (existed.IsFailure)
            return existed.Error;

        return predicate(existed.Value) ? existed : new Error("No elements satisfied");
    }

    public List<U> ResolveManyByCompare<U>(Func<U, bool> predicate)
    {
        if (Payload?.Data == null)
            return [];
        Type requestedType = typeof(U);
        List<object> elements = Payload
            .Data.Where(obj => obj.GetType().IsAssignableTo(requestedType))
            .ToList();
        return elements.Cast<U>().Where(predicate).ToList();
    }
}
