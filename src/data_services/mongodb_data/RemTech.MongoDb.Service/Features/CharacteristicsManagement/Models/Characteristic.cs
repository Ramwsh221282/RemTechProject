using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace RemTech.MongoDb.Service.Features.CharacteristicsManagement.Models;

public sealed class Characteristic
{
    [BsonGuidRepresentation(GuidRepresentation.Standard)]
    public Guid Id { get; private init; }
    public string Name { get; private init; }

    public Characteristic(string name)
    {
        Name = name;
        Id = Guid.NewGuid();
    }
}
