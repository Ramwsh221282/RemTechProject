using System.Text;
using Dapper;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisements.Specification;

public sealed class CustomSqlQuery
{
    private readonly StringBuilder _sqlBuilder;
    private readonly DynamicParameters _parameters;

    public CustomSqlQuery(StringBuilder sqlBuilder, DynamicParameters parameters)
    {
        _sqlBuilder = sqlBuilder;
        _parameters = parameters;
    }

    public string Sql => _sqlBuilder.ToString();

    public DynamicParameters Parameters => _parameters;

    public CustomSqlQuery AddPagination(PaginationOption pagination)
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

    public CustomSqlQuery AddSorting(SortMode? sortMode)
    {
        if (sortMode == null)
            return this;

        if (sortMode.Mode == "ASC")
            _sqlBuilder.AppendLine("ORDER BY ASCENDING");

        if (sortMode.Mode == "DESC")
            _sqlBuilder.AppendLine("ORDER BY DESCENDING");

        return this;
    }
}
