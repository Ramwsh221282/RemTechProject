namespace RemTech.Infrastructure.PostgreSql.Shared;

public sealed record PaginatedDaoResponse<T>(T[] Data, int Totals, int PagesCount)
{
    public static PaginatedDaoResponse<T> Create(
        IEnumerable<T> data,
        ref int totals,
        PaginationOptions pagination
    )
    {
        int pageSize = pagination.PageSize;
        int pagesCount = (int)Math.Ceiling((double)totals / pageSize);
        T[] dataResponse = [.. data];
        return new PaginatedDaoResponse<T>(dataResponse, totals, pagesCount);
    }
}
