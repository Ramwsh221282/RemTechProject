using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Common.Queries;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Queries;

internal sealed class TransportAdvertisementsQueryResolver
    : QueryResolver<FilterAdvertisementsDto, TransportAdvertisement>
{
    public TransportAdvertisementsQueryResolver(IServiceScopeFactory factory)
        : base(factory) { }
}
