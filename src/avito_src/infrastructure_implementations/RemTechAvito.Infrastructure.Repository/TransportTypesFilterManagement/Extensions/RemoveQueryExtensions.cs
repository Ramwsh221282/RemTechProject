using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Helpers;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Extensions;

public static class RemoveQueryExtensions
{
    public static FilterDefinition<TransportType> CreateFilter(
        this RemoveTransportTypeQuery transportTypeQuery
    )
    {
        return transportTypeQuery switch
        {
            RemoveSystemTransportTypeQuery => CreateSystemTypesRemoveFilter(),
            RemoveUserTransportTypeQuery userTypeQuery => CreateUserTypesRemoveFilter(
                userTypeQuery.Name,
                userTypeQuery.Link
            ),
            _ => Builders<TransportType>.Filter.Empty,
        };
    }

    private static FilterDefinition<TransportType> CreateSystemTypesRemoveFilter()
    {
        return FilterDefinition<TransportType>.Empty.AddEquality(
            "type_implementor",
            TransportType.SYSTEM_TYPE
        );
    }

    private static FilterDefinition<TransportType> CreateUserTypesRemoveFilter(
        string name,
        string link
    )
    {
        return FilterDefinition<TransportType>
            .Empty.AddEquality("type_implementor", TransportType.USER_TYPE)
            .AddEquality<TransportType>("type_name", name)
            .AddEquality("type_link", link);
    }
}
