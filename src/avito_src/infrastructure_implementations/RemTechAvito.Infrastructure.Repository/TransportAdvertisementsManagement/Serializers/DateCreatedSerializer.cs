﻿using MongoDB.Bson.Serialization;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechCommon.Utils.Converters;

namespace RemTechAvito.Infrastructure.Repository.TransportAdvertisementsManagement.Serializers;

internal sealed class DateCreatedSerializer : IBsonSerializer<DateCreated>
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
        DateCreated value
    )
    {
        var writer = context.Writer;
        writer.WriteDateTime(value.Date.ToUnixFromDateOnly());
    }

    public DateCreated Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
    {
        var reader = context.Reader;
        DateOnly date = reader.ReadDateTime().FromUnixToDateOnly();
        return new DateCreated(date);
    }

    public void Serialize(
        BsonSerializationContext context,
        BsonSerializationArgs args,
        object value
    )
    {
        Serialize(context, args, (DateCreated)value);
    }

    public Type ValueType => typeof(DateCreated);
}
