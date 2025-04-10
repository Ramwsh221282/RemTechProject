using RemTech.Infrastructure.PostgreSql.AdvertisementsContext.DaoModels;
using RemTech.Infrastructure.PostgreSql.Shared;

namespace RemTech.WebApi.AdvertisementsContext.Responses;

public sealed record class AdvertisementPaginatedResponse(
    AdvertisementResponse[] Advertisements,
    int Totals,
    int PagesCount
)
{
    public static AdvertisementPaginatedResponse Create(
        PaginatedDaoResponse<AdvertisementDao> response
    )
    {
        AdvertisementResponse[] data = [.. response.Data.Select(x => x.FromDao())];
        int totals = response.Totals;
        int pagesCount = response.PagesCount;
        return new AdvertisementPaginatedResponse(data, totals, pagesCount);
    }
}
