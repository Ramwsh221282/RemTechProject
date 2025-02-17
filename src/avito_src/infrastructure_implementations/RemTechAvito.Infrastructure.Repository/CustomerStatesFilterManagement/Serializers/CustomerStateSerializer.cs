using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization;
using RemTechAvito.Core.FiltersManagement.CustomerStates;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.CustomerStatesFilterManagement.Serializers;

public sealed class CustomerStateSerializer : IBsonSerializer<CustomerState>
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
        CustomerState value
    )
    {
        var writer = context.Writer;
        writer.WriteStartDocument();

        writer.WriteName("state_value");
        writer.WriteString(value.State);

        writer.WriteName("date_created");
        writer.WriteDateTime(value.CreatedOn.ToUnix());

        writer.WriteEndDocument();
    }

    public CustomerState Deserialize(
        BsonDeserializationContext context,
        BsonDeserializationArgs args
    )
    {
        var reader = context.Reader;
        reader.ReadStartDocument();

        string state = string.Empty;
        DateOnly date = DateOnly.MinValue;

        while (reader.CurrentBsonType != BsonType.EndOfDocument)
        {
            string property = reader.ReadName();
            switch (property)
            {
                case "state_value":
                    state = reader.ReadString();
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
        return CustomerState.Create(state, date);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (CustomerState)value);
    }

    public Type ValueType => typeof(CustomerState);
}
