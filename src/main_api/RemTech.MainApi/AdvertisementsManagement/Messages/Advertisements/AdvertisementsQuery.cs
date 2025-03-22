using RemTech.MainApi.Common.Dtos;

namespace RemTech.MainApi.AdvertisementsManagement.Messages.Advertisements;

public sealed record AdvertisementsQuery(
    AdvertisementQueryPayload Payload,
    PaginationOption Pagination
);
