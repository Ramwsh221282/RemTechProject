using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Driver;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement.ValueObjects;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.MongoDb.Tests.BasicCrudTests.Serializers.TransportAdvertisementManagement;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.MongoDb.Tests.BasicCrudTests;

public class Create_Entities_Tests
{
    private readonly ILogger _logger;

    private const string ConnectionString =
        "mongodb://root:example@localhost:27017/?authSource=admin";

    public Create_Entities_Tests()
    {
        _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
    }

    [Fact]
    public async Task Test_Simple_List_Databases()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        using MongoClient client = new MongoClient(ConnectionString);
        CancellationToken ct = cts.Token;

        try
        {
            using (var cursor = await client.ListDatabasesAsync())
            {
                var databases = cursor.ToEnumerable();
                foreach (var database in databases)
                {
                    var name = database["name"].AsString;
                    var sizeOnDisk = database["sizeOnDisk"].AsInt64;
                    var isEmpty = database["empty"].AsBoolean;

                    _logger.Information(
                        "Db. Name: {Db}. Size: {Size}. Is Empty: {IsEmpty}",
                        name,
                        sizeOnDisk,
                        isEmpty
                    );
                }
            }
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Exception}", ex.Message);
        }
    }

    [Fact]
    public async Task Test_Simple_Create_Custom_Database()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        using MongoClient client = new MongoClient(ConnectionString);
        CancellationToken ct = cts.Token;

        try
        {
            IMongoDatabase db = client.GetDatabase("my_custom");
            await db.CreateCollectionAsync("my_custom_collection", cancellationToken: ct);

            List<string> databaseNames = [];
            using (var cursor = await client.ListDatabasesAsync())
            {
                var databases = cursor.ToEnumerable();
                foreach (var database in databases)
                {
                    var name = database["name"].AsString;
                    _logger.Information("Db. Name: {Db}.", name);
                    databaseNames.Add(name);
                }
            }
            Assert.Contains("my_custom", databaseNames);

            await client.DropDatabaseAsync("my_custom", ct);

            databaseNames = [];
            using (var cursor = await client.ListDatabasesAsync())
            {
                var databases = cursor.ToEnumerable();
                foreach (var database in databases)
                {
                    var name = database["name"].AsString;
                    _logger.Information("Db. Name: {Db}.", name);
                    databaseNames.Add(name);
                }
            }
            Assert.DoesNotContain("my_custom", databaseNames);
        }
        catch (Exception ex)
        {
            _logger.Fatal("{Exception}", ex.Message);
        }
    }

    [Fact]
    public async Task Create_Advertisement_Data_Mapper()
    {
        bool noExceptions = true;

        try
        {
            BsonSerializer.RegisterSerializer(new AddressSerializer());
            BsonSerializer.RegisterSerializer(new AdvertisementIDSerializer());
            BsonSerializer.RegisterSerializer(new CharacteristicsSerializer());
            BsonSerializer.RegisterSerializer(new DescriptionSerializer());
            BsonSerializer.RegisterSerializer(new EntityIDSerializer());
            BsonSerializer.RegisterSerializer(new OwnerInformationSerializer());
            BsonSerializer.RegisterSerializer(new PhotoAttachmentsSerializer());
            BsonSerializer.RegisterSerializer(new PriceSerializer());
            BsonSerializer.RegisterSerializer(new TitleSerializer());
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal("Mapper error: {Exception}", ex.Message);
        }

        using CancellationTokenSource cts = new CancellationTokenSource();
        using MongoClient client = new MongoClient(ConnectionString);
        CancellationToken ct = cts.Token;

        try
        {
            IMongoDatabase db = client.GetDatabase("my_custom");
            await db.CreateCollectionAsync("my_custom_collection", cancellationToken: ct);

            var collection = db.GetCollection<TransportAdvertisement>("my_custom_collection");

            Result<AdvertisementID> id = AdvertisementID.Create(123123);
            Assert.True(id.IsSuccess);
            Result<Characteristics> characteristic = new Characteristics(
                [Characteristic.Create("age", "20")]
            );
            Assert.True(characteristic.IsSuccess);
            Result<Address> address = Address.Create("US 32 A");
            Assert.True(address.IsSuccess);
            Result<OwnerInformation> ownerInfo = OwnerInformation.Create("By nobody", "Nobody");
            Assert.True(ownerInfo.IsSuccess);
            Result<PhotoAttachments> photoAttachments = new PhotoAttachments(
                [Photo.Create("url:123"), Photo.Create("url:456")]
            );
            Assert.True(photoAttachments.IsSuccess);
            Result<Price> price = Price.Create(100000, "USD", "My money");
            Assert.True(price.IsSuccess);
            Result<Title> title = Title.Create("This is my title");
            Assert.True(title.IsSuccess);
            Result<Description> description = Description.Create("Hello my name is John");
            Assert.True(description.IsSuccess);

            TransportAdvertisement advertisement = new TransportAdvertisement(
                id,
                characteristic,
                address,
                ownerInfo,
                photoAttachments,
                price,
                title,
                description
            );

            await collection.InsertOneAsync(advertisement, cancellationToken: ct);

            await client.DropDatabaseAsync("my_custom", ct);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal("{Exception}", ex.Message);
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Simple_Insert_Single_Advertisement_Test()
    {
        Result<AdvertisementID> id = AdvertisementID.Create(123123);
        Assert.True(id.IsSuccess);
        Result<Characteristics> characteristic = new Characteristics(
            [Characteristic.Create("age", "20")]
        );
        Assert.True(characteristic.IsSuccess);
        Result<Address> address = Address.Create("US 32 A");
        Assert.True(address.IsSuccess);
        Result<OwnerInformation> ownerInfo = OwnerInformation.Create("By nobody", "Nobody");
        Assert.True(ownerInfo.IsSuccess);
        Result<PhotoAttachments> photoAttachments = new PhotoAttachments(
            [Photo.Create("url:123"), Photo.Create("url:456")]
        );
        Assert.True(photoAttachments.IsSuccess);
        Result<Price> price = Price.Create(100000, "USD", "My money");
        Assert.True(price.IsSuccess);
        Result<Title> title = Title.Create("This is my title");
        Assert.True(title.IsSuccess);
        Result<Description> description = Description.Create("Hello my name is John");
        Assert.True(description.IsSuccess);

        TransportAdvertisement advertisement = new TransportAdvertisement(
            id,
            characteristic,
            address,
            ownerInfo,
            photoAttachments,
            price,
            title,
            description
        );
    }
}
