using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Abstractions.TextSearchFilterFactory;
using RemTechCommon.Utils.Extensions;

namespace RemTech.MongoDb.Service.Common.Dtos;

public record TextSearchOption(string? SearchTerm = null);

public sealed record FormattedSearchOption(string SearchTerm) : TextSearchOption(SearchTerm);

public sealed record EmptyTextSearchOption() : TextSearchOption(string.Empty);

public static class TextSearchOptionExtensions
{
    public static TextSearchOption Specify(this TextSearchOption option)
    {
        string? term = option.SearchTerm;
        string formatted = term.CleanString();
        return formatted switch
        {
            not null when formatted.Length == 0 => new EmptyTextSearchOption(),
            not null when formatted.Length > 0 => new FormattedSearchOption(formatted),
            _ => new EmptyTextSearchOption(),
        };
    }

    private static FilterDefinition<TEntity> ApplyTextSearch<TEntity>(
        this TextSearchOption option,
        FilterDefinition<TEntity> filter,
        ITextSearchFilterFactory<TEntity> factory
    )
        where TEntity : class =>
        option switch
        {
            EmptyTextSearchOption => filter,
            FormattedSearchOption => factory.Apply(filter, option),
            _ => filter,
        };

    public static FilterDefinition<TEntity> ApplyTextSearch<TEntity>(
        this FilterDefinition<TEntity> filter,
        TextSearchOption option,
        ITextSearchFilterFactory<TEntity> factory
    )
        where TEntity : class => option.ApplyTextSearch(filter, factory);
}
