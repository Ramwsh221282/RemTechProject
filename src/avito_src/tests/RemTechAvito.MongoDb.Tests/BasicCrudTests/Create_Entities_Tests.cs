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
}
