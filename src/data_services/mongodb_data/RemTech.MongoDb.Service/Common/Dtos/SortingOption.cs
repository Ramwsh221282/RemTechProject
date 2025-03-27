using System.Linq.Expressions;
using MongoDB.Driver;

namespace RemTech.MongoDb.Service.Common.Dtos;

public record SortingOption(string Option)
{
    public virtual IFindFluent<T, T> Accept<T>(
        IFindFluent<T, T> find,
        Expression<Func<T, object>> expression
    )
    {
        return find;
    }
}

public sealed record AscendingSortingOption() : SortingOption("ASC")
{
    public override IFindFluent<T, T> Accept<T>(
        IFindFluent<T, T> find,
        Expression<Func<T, object>> expression
    ) => base.Accept(find.SortBy(expression), expression);
}

public sealed record DescendingSortingOption() : SortingOption("DESC")
{
    public override IFindFluent<T, T> Accept<T>(
        IFindFluent<T, T> find,
        Expression<Func<T, object>> expression
    ) => base.Accept(find.SortByDescending(expression), expression);
}

public sealed record UnspecifiedSortingOption() : SortingOption("NONE");

public static class SortingOptionExtensions
{
    public static SortingOption Specify(this SortingOption option) =>
        option.Option switch
        {
            "ASC" => new AscendingSortingOption(),
            "DESC" => new DescendingSortingOption(),
            _ => new UnspecifiedSortingOption(),
        };

    public static IFindFluent<T, T> Accept<T>(
        this IFindFluent<T, T> find,
        Expression<Func<T, object>> expression,
        SortingOption? option
    ) => option == null ? find : option.Accept(find, expression);
}
