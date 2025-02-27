using MongoDB.Driver;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
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
                userTypeQuery.Name
            ),
            _ => Builders<TransportType>.Filter.Empty,
        };
    }

    private static FilterDefinition<TransportType> CreateSystemTypesRemoveFilter()
    {
        return FilterDefinitionFactory.CreateEquality<TransportType>(
            "type_implementor",
            TransportType.SYSTEM_TYPE
        );
    }

    private static FilterDefinition<TransportType> CreateUserTypesRemoveFilter(string name)
    {
        return FilterDefinitionFactory
            .CreateEquality<TransportType>("type_implementor", TransportType.USER_TYPE)
            .CreateEquality("type_name", name);
    }
}
