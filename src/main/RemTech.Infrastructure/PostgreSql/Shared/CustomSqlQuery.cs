using System.Text;
using Dapper;

namespace RemTech.Infrastructure.PostgreSql.Shared;

public sealed class CustomSqlQuery(StringBuilder sqlBuilder, DynamicParameters parameters)
{
    private readonly StringBuilder _sqlBuilder = sqlBuilder;
    private readonly DynamicParameters _parameters = parameters;

    public string Sql => _sqlBuilder.ToString();

    public DynamicParameters Parameters => _parameters;

    public CustomSqlQuery AddPagination(PaginationOptions pagination)
    {
        int page = pagination.Page;
        int pageSize = pagination.PageSize;
        int offset = (page - 1) * pageSize;
        int limit = pageSize;

        _sqlBuilder.AppendLine("OFFSET @offset");
        _sqlBuilder.AppendLine("LIMIT @limit");
        _parameters.Add("@offset", offset);
        _parameters.Add("@limit", limit);

        return this;
    }

    public CustomSqlQuery AddSorting(SortOptions? sortMode, string columnName)
    {
        if (sortMode == null)
            return this;

        if (sortMode.Mode == "ASC")
            _sqlBuilder.AppendLine($"ORDER BY {columnName} ASC");

        if (sortMode.Mode == "DESC")
            _sqlBuilder.AppendLine($"ORDER BY {columnName} DESC");

        return this;
    }
}
