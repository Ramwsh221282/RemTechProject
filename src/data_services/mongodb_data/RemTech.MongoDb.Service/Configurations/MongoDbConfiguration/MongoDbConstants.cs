using MongoDB.Driver;
using RemTech.MongoDb.Service.Common.Models.AdvertisementsManagement;
using RemTech.MongoDb.Service.Common.Models.ParsersManagement;
using RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;

namespace RemTech.MongoDb.Service.Configurations.MongoDbConfiguration;

public static class MongoDbConstants
{
    private const string AdvertisementsDb = "Advertisements";
    private const string AdvertisementsCol = "Advertisements_Collection";
    private const string CharacteristicsCol = "Advertisements_Characteristics_Collection";
    private const string ParsersDb = "Parsers";
    private const string ParsersCol = "Parsers_Collection";

    public static IMongoDatabase GetAdvertisementsDb(this MongoClient client) =>
        client.GetDatabase(AdvertisementsDb);

    public static IMongoCollection<Advertisement> GetAdvertisementsCol(this IMongoDatabase db) =>
        db.GetCollection<Advertisement>(AdvertisementsCol);

    public static IMongoDatabase GetParsersDb(this MongoClient client) =>
        client.GetDatabase(ParsersDb);

    public static IMongoCollection<Parser> GetParsersCol(this IMongoDatabase db) =>
        db.GetCollection<Parser>(ParsersCol);

    public static IMongoCollection<Characteristic> GetCharacteristicsCol(this IMongoDatabase db) =>
        db.GetCollection<Characteristic>(CharacteristicsCol);
}
