using System.Text.Json;
using System.Text.RegularExpressions;
using RemTech.Shared.SDK.ResultPattern;
using SharedParsersLibrary.Models;

namespace RemTech.Parser.Drom.Scraping.Models;

public abstract record DromTransportAttributeType : IDromScrapedAdvertisementProperty
{
    public const string Plain = "Plain";
    public const string Power = "Power";
    public const string SpecFrameType = "SpecFrameType";
    public const string HasMileage = "HasMileage";
    public const string Mileage = "Mileage";
    public const string VolumeWithLabel = "VolumeWithLabel";
    public const string SpecialDescription = "SpecialDescription";
    public const string Frame = "Frame";

    public abstract ScrapedAdvertisement Set(ScrapedAdvertisement advertisement);
}

public sealed record PlainAttributeType(string Label, string Value) : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new(Label, Value);
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public sealed record PowerAttributeType(int PowerValue) : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new("Мощность", $"{PowerValue} л.с.");
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public sealed record SpecFrameTypeAttributeType(string Label, string Value)
    : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new(Label, Value);
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public sealed record MileageAttributeType(string Label, string Marks) : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new(Label, Marks);
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public sealed record VolumeWithLabelAttributeType(string Label, string Value)
    : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new(Label, Value);
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public sealed record FrameAttributeType(bool IsVin, string FrameNumber) : DromTransportAttributeType
{
    public override ScrapedAdvertisement Set(ScrapedAdvertisement advertisement)
    {
        ScrapedCharacteristic ctx = new("VIN", FrameNumber.Replace("*", ""));
        ScrapedCharacteristic[] array = [ctx, .. advertisement.Characteristics];
        return advertisement with { Characteristics = array };
    }
}

public static class DromTransportAttributeTypeFactory
{
    public static ScrapedAdvertisement Set(
        this DromTransportAttributeType[] types,
        ScrapedAdvertisement advertisement
    ) => types.Aggregate(advertisement, (current, type) => type.Set(current));

    public static DromTransportAttributeType[] CreateFromJsonDocument(JsonDocument document)
    {
        bool hasDescription = document.RootElement.TryGetProperty(
            "bullDescription",
            out JsonElement boolDescription
        );

        if (!hasDescription)
            return [];

        bool hasFields = boolDescription.TryGetProperty("fields", out JsonElement fields);

        if (!hasFields)
            return [];

        IEnumerable<Result<DromTransportAttributeType>> results = fields
            .EnumerateArray()
            .Select(Create)
            .Where(r => r.IsSuccess);

        DromTransportAttributeType[] types = [.. results];

        return types;
    }

    public static Result<DromTransportAttributeType> Create(JsonElement field)
    {
        bool hasType = field.TryGetProperty("type", out JsonElement typeProperty);
        if (!hasType)
            return new Error("Field property was not found");
        string? typeName = typeProperty.GetString();
        if (string.IsNullOrWhiteSpace(typeName))
            return new Error("Type name is empty");

        return typeName switch
        {
            null => new Error("Type name property is null"),
            not null when string.IsNullOrWhiteSpace(typeName) => new Error(
                "Type name property is empty"
            ),
            not null when typeName.IsMatch(DromTransportAttributeType.Plain) =>
                AsPlainAttributeType(field),
            not null when typeName.IsMatch(DromTransportAttributeType.Frame) =>
                AsFrameAttributeType(field),
            not null when typeName.IsMatch(DromTransportAttributeType.Mileage) =>
                AsMileageAttributeType(field),
            not null when typeName.IsMatch(DromTransportAttributeType.Power) =>
                AsPowerAttributeType(field),
            not null when typeName.IsMatch(DromTransportAttributeType.SpecFrameType) =>
                AsSpecFrameAttributeType(field),
            not null when typeName.IsMatch(DromTransportAttributeType.VolumeWithLabel) =>
                AsVolumeWithLabelAttributeType(field),
            _ => new Error("No matching properties found"),
        };
    }

    private static Result<DromTransportAttributeType> AsPlainAttributeType(this JsonElement field)
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        string? label = payload.GetProperty("label").GetString();
        if (string.IsNullOrWhiteSpace(label))
            return new Error("Label is empty");
        string? value = payload.GetProperty("value").GetString();
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Value is empty");
        return new PlainAttributeType(Regex.Unescape(label), Regex.Unescape(value));
    }

    private static Result<DromTransportAttributeType> AsPowerAttributeType(this JsonElement field)
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        bool hasPower = payload.TryGetProperty("power", out JsonElement powerElement);
        if (!hasPower)
            return new Error("Power element is empty");
        int powerValue = powerElement.GetInt32();
        return new PowerAttributeType(powerValue);
    }

    private static Result<DromTransportAttributeType> AsSpecFrameAttributeType(
        this JsonElement field
    )
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        string? label = payload.GetProperty("label").GetString();
        if (string.IsNullOrWhiteSpace(label))
            return new Error("Label is empty");
        string? value = payload.GetProperty("value").GetString();
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Value is empty");
        return new SpecFrameTypeAttributeType(Regex.Unescape(label), Regex.Unescape(value));
    }

    private static Result<DromTransportAttributeType> AsMileageAttributeType(this JsonElement field)
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        string? label = payload.GetProperty("label").GetString();
        if (string.IsNullOrWhiteSpace(label))
            return new Error("Label is empty");
        bool hasMileageValueProperty = payload.TryGetProperty(
            "mark",
            out JsonElement mileageValueProperty
        );
        if (!hasMileageValueProperty)
            return new Error("Mark property is empty");
        if (mileageValueProperty.ValueKind == JsonValueKind.Null)
            return new MileageAttributeType(Regex.Unescape(label), "Новый");
        int mileageValue = mileageValueProperty.GetInt32();
        return new MileageAttributeType(Regex.Unescape(label), mileageValue.ToString());
    }

    private static Result<DromTransportAttributeType> AsVolumeWithLabelAttributeType(
        this JsonElement field
    )
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        string? label = payload.GetProperty("label").GetString();
        if (string.IsNullOrWhiteSpace(label))
            return new Error("Label is empty");
        string? value = payload.GetProperty("value").GetString();
        if (string.IsNullOrWhiteSpace(value))
            return new Error("Value is empty");
        return new VolumeWithLabelAttributeType(Regex.Unescape(label), Regex.Unescape(value));
    }

    private static Result<DromTransportAttributeType> AsFrameAttributeType(this JsonElement field)
    {
        Result<JsonElement> payloadResult = field.GetPayload();
        if (payloadResult.IsFailure)
            return payloadResult.Error;
        JsonElement payload = payloadResult.Value;
        bool hasIsVinProperty = payload.TryGetProperty("isVin", out JsonElement isVinProperty);
        if (!hasIsVinProperty)
            return new Error("Is Vin Property is empty");
        bool hasFrameNumberProperty = payload.TryGetProperty(
            "frameNumber",
            out JsonElement frameNumberProperty
        );
        if (!hasFrameNumberProperty)
            return new Error("Frame number property is empty");
        string? frameNumberValue = frameNumberProperty.GetString();
        if (string.IsNullOrWhiteSpace(frameNumberValue))
            return new Error("Frame number value is empty");
        bool isVinValue = isVinProperty.GetBoolean();
        return new FrameAttributeType(isVinValue, frameNumberValue);
    }

    private static bool IsMatch(this string input, string pattern) =>
        input.Equals(pattern, StringComparison.OrdinalIgnoreCase);

    private static Result<JsonElement> GetPayload(this JsonElement field)
    {
        bool hasPayload = field.TryGetProperty("payload", out JsonElement payload);
        if (!hasPayload)
            return new Error("Payload does not exists");
        return payload;
    }
}
