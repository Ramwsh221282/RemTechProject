using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportTypes;
using RemTechAvito.Infrastructure.Repository.Common.Indexes;

namespace RemTechAvito.Infrastructure.Repository.TransportTypesFilterManagement.Indexes;

internal sealed class TransportTypesIndexModel : AbstractIndexModel<TransportType>
{
    public TransportTypesIndexModel()
    {
        Add(
            new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Text("type_name"),
                new CreateIndexOptions() { Name = "Transport_Types_Text_Search" }
            )
        );
        Add(
            new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Ascending("type_implementor"),
                new CreateIndexOptions() { Name = "Transport_Types_Type_Implementor" }
            )
        );
        Add(
            new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Ascending("type_name"),
                new CreateIndexOptions() { Name = "Transport_Types_Type_Name" }
            )
        );
        Add(
            new CreateIndexModel<TransportType>(
                Builders<TransportType>.IndexKeys.Ascending("type_link"),
                new CreateIndexOptions() { Name = "Transport_Types_Type_Link", Unique = true }
            )
        );
    }
}
