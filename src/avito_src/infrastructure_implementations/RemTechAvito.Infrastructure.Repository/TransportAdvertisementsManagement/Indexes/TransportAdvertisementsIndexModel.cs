using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Infrastructure.Repository.Common.Indexes;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Indexes;

internal sealed class TransportAdvertisementsIndexModel : AbstractIndexModel<TransportAdvertisement>
{
    public TransportAdvertisementsIndexModel()
    {
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Text("Description")
                    .Text("Title")
                    .Text("CharacteristicsSearch.attribute_name")
                    .Text("CharacteristicsSearch.attribute_value"),
                new CreateIndexOptions() { Name = "Description_Title_Characteristics_Text" }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>.IndexKeys.Ascending("AdvertisementId"),
                new CreateIndexOptions() { Name = "AdvertisementId_Index", Unique = true }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("Price.price_value")
                    .Ascending("Price.price_currency")
                    .Ascending("Price.price_extra"),
                new CreateIndexOptions { Name = "Price_Value_Currency_Extra" }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>.IndexKeys.Ascending("CreatedOn"),
                new CreateIndexOptions { Name = "CreatedOn_Index" }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("CharacteristicsSearch.attribute_name")
                    .Ascending("CharacteristicsSearch.attribute_value"),
                new CreateIndexOptions { Name = "Characteristics_Name_Value" }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>
                    .IndexKeys.Ascending("Owner.owner_description")
                    .Ascending("Owner.owner_status")
                    .Ascending("Owner.owner_contacts"),
                new CreateIndexOptions { Name = "OwnerInformation_Description_Status_Contacts" }
            )
        );
        Add(
            new CreateIndexModel<TransportAdvertisement>(
                Builders<TransportAdvertisement>.IndexKeys.Ascending("Address"),
                new CreateIndexOptions() { Name = "Address_Index" }
            )
        );
    }
}
