using RemTech.MainApi.AdvertisementsManagement.Dtos;
using RemTech.MainApi.AdvertisementsManagement.Models;
using RemTech.MainApi.Common.Abstractions;
using RemTech.MainApi.Common.Dtos;
using RemTechCommon.Utils.ResultPattern;

namespace RemTech.MainApi.AdvertisementsManagement.Features.GetAdvertisements;

public sealed record GetAdvertisementsQuery(
    AdvertisementsFilterOption Option,
    PaginationOption Pagination
) : IQuery<Result<(TransportAdvertisement[] advertisements, long count)>>;
