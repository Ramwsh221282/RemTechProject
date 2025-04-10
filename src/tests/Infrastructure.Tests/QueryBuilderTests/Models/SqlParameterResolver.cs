using System.Text;
using System.Text.RegularExpressions;
using Dapper;

namespace Infrastructure.Tests.QueryBuilderTests.Models;

public static partial class SqlParameterResolver
{
    private static readonly Regex _columnNameParseRegex = ColumnNameExtractorRegex();
    private static readonly Regex _parameterNameParseRegex = ParameterNameExtractorRegex();

    public static DynamicParameters Resolve<TParameter>(
        this DynamicParameters parameters,
        string sql,
        TParameter parameter
    )
    {
        parameters = sql switch
        {
            not null when sql.Contains("LIKE") => parameters.AppendLike(sql, parameter),
            not null when sql.Contains('~') => parameters.AppendRegex(sql, parameter),
            _ => parameters.AppendRaw(sql!, parameter),
        };

        return parameters;
    }

    public static DynamicParameters Resolve<TParameter>(
        this DynamicParameters parameters,
        string sql,
        (TParameter, string)[] items
    )
    {
        foreach ((TParameter, string) parameter in items)
        {
            parameters = parameters.Resolve(sql, parameter);
        }

        return parameters;
    }

    private static DynamicParameters Resolve<TParameter>(
        this DynamicParameters parameters,
        string sql,
        (TParameter, string) parameter
    )
    {
        parameters = sql switch
        {
            not null when sql.Contains("LIKE") => parameters.AppendLike(
                sql,
                parameter.Item1,
                parameter.Item2
            ),
            not null when sql.Contains('~') => parameters.AppendRegex(
                sql,
                parameter.Item1,
                parameter.Item2
            ),
            _ => parameters.AppendRaw(sql!, parameter),
        };

        return parameters;
    }

    private static DynamicParameters AppendRegex<TParameter>(
        this DynamicParameters parameters,
        string sql,
        TParameter parameter,
        string? parameterNameArg = null
    )
    {
        if (typeof(TParameter) != typeof(string))
            throw new ArgumentException($"Regex parameter should be string: {sql}");

        string parameterString = (parameter as string)!;
        string regex = $"(?i).*{parameterString}.*";
        string parameterName = parameterNameArg ?? sql.GetParameterName();

        parameters.Add(parameterName, regex);
        return parameters;
    }

    private static DynamicParameters AppendLike<TParameter>(
        this DynamicParameters parameters,
        string sql,
        TParameter parameter,
        string? parameterNameArg = null
    )
    {
        if (typeof(TParameter) != typeof(string))
            throw new ArgumentException($"Like parameter should be string: {sql}");

        string parameterString = (parameter as string)!;

        StringBuilder parameterBuilder = new();
        parameterBuilder.Append('%');
        parameterBuilder.Append(parameterString);
        parameterBuilder.Append('%');

        string parameterName = parameterNameArg ?? sql.GetParameterName();
        parameters.Add(parameterName, parameterBuilder.ToString());
        return parameters;
    }

    private static DynamicParameters AppendRaw<TParameter>(
        this DynamicParameters parameters,
        string sql,
        TParameter parameter
    )
    {
        string parameterName = sql.GetParameterName();
        parameters.Add(parameterName, parameter);
        return parameters;
    }

    public static string GetParameterName(this string sql)
    {
        Match _match = _parameterNameParseRegex.Match(sql);

        if (!_match.Success)
            throw new ArgumentException($"Cannot find parameter name in sql: {sql}");

        return _match.Groups[1].Value;
    }

    [GeneratedRegex(@"\b([A-Z]\w*)\s+([A-Z]\w*)", RegexOptions.Compiled)]
    private static partial Regex ColumnNameExtractorRegex();

    [GeneratedRegex(@"(@\w+)\b", RegexOptions.Compiled)]
    private static partial Regex ParameterNameExtractorRegex();
}
