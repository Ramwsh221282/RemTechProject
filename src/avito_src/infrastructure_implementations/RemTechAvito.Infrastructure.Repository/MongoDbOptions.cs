namespace RemTechAvito.Infrastructure.Repository;

public sealed class MongoDbOptions
{
    public string ConnectionString { get; set; } =
        "mongodb://root:example@localhost:27017/?authSource=admin";
}
