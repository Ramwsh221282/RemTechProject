using System.Text;
using Dapper;

namespace Infrastructure.Tests.QueryBuilderTests.Models;

public sealed class CustomGenericQueryBuilder
{
    private readonly StringBuilder _builder;
    private readonly DynamicParameters _parameters;
    private QueryStringState _queryState;
    private bool _whereAdded;

    public string Sql => _builder.ToString();
    public DynamicParameters Parameters => _parameters;

    public CustomGenericQueryBuilder()
    {
        _builder = new StringBuilder();
        _parameters = new DynamicParameters();
        _queryState = QueryStringState.NotStarted;
        _whereAdded = false;
    }

    public CustomGenericQueryBuilder Select(params string[] selectTerm)
    {
        _builder.Append($"SELECT ");
        foreach (string term in selectTerm)
        {
            _builder.Append($"{term} ");
        }
        _queryState = QueryStringState.Select;
        return this;
    }

    public CustomGenericQueryBuilder From(string table)
    {
        _builder.Append($"FROM {table} ");
        _queryState = QueryStringState.From;
        return this;
    }

    public CustomGenericQueryBuilder WhereIf<T, U>(
        T value,
        Func<T, bool> predicateFn,
        Func<T, U> parameterFn,
        string query
    )
    {
        if (!predicateFn(value))
            return this;

        string parameterName = query.GetParameterName();
        U parameterObject = parameterFn(value);
        _parameters.Add(parameterName, parameterObject);

        if (_whereAdded)
        {
            _builder.Append($"AND {query} ");
            _queryState = QueryStringState.And;
            return this;
        }

        _builder.Append($"WHERE {query} ");
        _queryState = QueryStringState.Where;
        _whereAdded = true;
        return this;
    }
}
