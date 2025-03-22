using RemTech.MongoDb.Service.Common.Dtos;

namespace RemTech.MongoDb.Service.Features.AdvertisementsManagement.AdvertisementQuerying;

public sealed record AdvertisementsQuery(
    AdvertisementQueryPayload Payload,
    PaginationOption Pagination
);
