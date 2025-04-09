using System.Text;
using Dapper;
using RemTech.Shared.SDK.CqrsPattern.Queries;
using RemTech.Shared.SDK.Utils;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;

public class GenericSpecificationBuilder<TQuery> : ISpecificationBuilder
    where TQuery : IQuery
{
    private readonly StringBuilder _queryBuilder;
    private readonly DynamicParameters _parameters;
    private readonly TQuery _query;

    public GenericSpecificationBuilder(string selectSql, TQuery query)
    {
        _queryBuilder = new StringBuilder(selectSql);
        _parameters = new DynamicParameters();
        _query = query;
    }

    private GenericSpecificationBuilder(
        StringBuilder queryBuilder,
        TQuery query,
        DynamicParameters parameters
    )
    {
        _queryBuilder = queryBuilder;
        _query = query;
        _parameters = parameters;
    }

    private GenericSpecificationBuilder(GenericSpecificationBuilder<TQuery> builder)
        : this(builder._queryBuilder, builder._query, builder._parameters) { }

    public GenericSpecificationBuilder<TQuery> Append(string sql)
    {
        _queryBuilder.AppendLine(sql);
        return new GenericSpecificationBuilder<TQuery>(this);
    }

    public GenericSpecificationBuilder<TQuery> AppendFilterIf<TParameter>(
        string sql,
        Func<TQuery, bool> predicate,
        Func<TQuery, TParameter> parameterFn
    )
    {
        if (!predicate(_query))
            return this;

        GenericSpecificationBuilder<TQuery> updated = Append(sql);
        TParameter parameter = parameterFn(_query);
        DynamicParameters parameters = _parameters.Resolve(sql, parameter);
        return new GenericSpecificationBuilder<TQuery>(
            updated._queryBuilder,
            updated._query,
            parameters
        );
    }

    public GenericSpecificationBuilder<TQuery> AppendTextSearchIf(
        Func<TQuery, bool> predicate,
        Func<TQuery, string> termString,
        string[] columns
    )
    {
        if (!predicate(_query))
            return this;

        string searchTerm = termString(_query).CleanString();

        string[] words = searchTerm.Contains(' ')
            ? searchTerm.Split(' ', StringSplitOptions.RemoveEmptyEntries)
            : [searchTerm];

        List<string> searchConditions = [];

        for (int i = 0; i < words.Length; i++)
        {
            _parameters.Add($"@word{i}", words[i]);

            foreach (string column in columns)
            {
                searchConditions.Add($"({column} ILIKE CONCAT('%', @word{i}, '%'))");
            }
        }

        string combinedConditions = string.Join(" OR ", searchConditions);
        _queryBuilder.Append($"AND ({combinedConditions})");

        return this;
    }

    public GenericSpecificationBuilder<TQuery> Append<TParameter>(string sql, TParameter parameter)
    {
        DynamicParameters parameters = _parameters.Resolve(sql, parameter);
        _queryBuilder.AppendLine(sql);
        return new GenericSpecificationBuilder<TQuery>(_queryBuilder, _query, parameters);
    }

    public GenericSpecificationBuilder<TQuery> AppendRawIf(
        string sql,
        Func<TQuery, bool> predicate
    ) => !predicate(_query) ? this : Append(sql);

    public TSpecificationBuilder AdaptTo<TSpecificationBuilder>(
        Func<
            StringBuilder,
            DynamicParameters,
            ISpecificationBuilder<TSpecificationBuilder>
        > factoryFn
    )
        where TSpecificationBuilder : ISpecificationBuilder<TSpecificationBuilder>
    {
        ISpecificationBuilder<TSpecificationBuilder> builder = factoryFn(
            _queryBuilder,
            _parameters
        );
        return builder.Instance;
    }

    public CustomSqlQuery Create() => new CustomSqlQuery(_queryBuilder, _parameters);
}
