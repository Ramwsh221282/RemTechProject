using RemTech.Shared.SDK.CqrsPattern.Queries;

namespace RemTech.Infrastructure.PostgreSql.AdvertisementsContext.Queries.GetAdvertisementById;

public sealed record GetAdvertisementByIdQuery(long Id) : IQuery;
