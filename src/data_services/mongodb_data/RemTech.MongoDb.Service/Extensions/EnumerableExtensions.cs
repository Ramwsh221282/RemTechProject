namespace RemTech.MongoDb.Service.Extensions;

public sealed record EnumerableChunk<T>(IEnumerable<T> Enumerable);

public static class EnumerableExtensions
{
    public static EnumerableChunk<T> AsChunk<T>(this IEnumerable<T> enumerable) => new(enumerable);

    public static EnumerableChunk<U> Map<T, U>(
        this EnumerableChunk<T> chunk,
        Func<T, U> mappingFunc
    ) => new(chunk.Enumerable.Select(item => mappingFunc(item)));

    public static T[] AsArray<T>(this EnumerableChunk<T> chunk) => [.. chunk.Enumerable];

    public static List<T> AsList<T>(this EnumerableChunk<T> chunk) => [.. chunk.Enumerable];
}
