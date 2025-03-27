using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Extensions;

public record FindFluentWhen<T>(IFindFluent<T, T> Find, bool Satisfied);

public record FindFluentWhenWithCount<T>(FindFluentWhen<T> When, long Count)
    : FindFluentWhen<T>(When);

public static class FindFluentExtensions
{
    public static FindFluentWhen<T> When<T>(this IFindFluent<T, T> find, bool satisfied) =>
        new(find, satisfied);

    public static FindFluentWhen<T> Apply<T>(
        this FindFluentWhen<T> find,
        Func<IFindFluent<T, T>, IFindFluent<T, T>> func
    ) => find.Satisfied ? find with { Find = func(find.Find) } : find;

    public static async Task<FindFluentWhenWithCount<T>> WithCount<T>(
        this FindFluentWhen<T> find
    ) => !find.Satisfied ? new(find, 0) : new(find, await find.Find.CountDocumentsAsync());

    public static async Task<List<T>> AsList<T>(this FindFluentWhen<T> when) =>
        when.Satisfied ? await when.Find.ToListAsync() : [];
}
