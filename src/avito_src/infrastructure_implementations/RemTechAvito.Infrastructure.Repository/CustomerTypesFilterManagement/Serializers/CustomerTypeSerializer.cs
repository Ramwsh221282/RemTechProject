using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.FiltersManagement.CustomerTypes;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.CustomerTypesFilterManagement.Serializers;

internal sealed class CustomerTypeSerializer : IBsonSerializer<CustomerType>
{
    object IBsonSerializer.Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        return Deserialize(context, args);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        CustomerType value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();

        writer.WriteName("type_name");
        writer.WriteString(value.Type);

        writer.WriteName("date_created");
        writer.WriteDateTime(value.CreatedOn.ToUnix());

        writer.WriteEndDocument();
    }

    public CustomerType Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;

        string type = string.Empty;
        DateOnly date = default;

        reader.ReadStartDocument();
        while (reader.CurrentBsonType != BsonType.EndOfDocument)
        {
            string property = reader.ReadName();
            switch (property)
            {
                case "type_name":
                    type = reader.ReadString();
                    break;
                case "date_created":
                    date = reader.ReadDateTime().FromUnix();
                    break;
                default:
                    reader.SkipValue();
                    break;
            }
        }
        reader.ReadEndDocument();
        return CustomerType.Create(type, date);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (CustomerType)value);
    }

    public Type ValueType => typeof(CustomerType);
}
