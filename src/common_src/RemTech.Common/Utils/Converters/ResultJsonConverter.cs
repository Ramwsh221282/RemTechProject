using System.Text.Json;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechCommon.Utils.Converters;

public static class ResultJsonConverter
{
    public static Result Convert(string json)
    {
        try
        {
            using JsonDocument document = JsonDocument.Parse(json);
            document.RootElement.TryGetProperty("IsSuccess", out JsonElement isSuccessProperty);
            bool isPropertyValueSuccess = isSuccessProperty.GetBoolean();
            if (isPropertyValueSuccess)
                return Result.Success();
            document.RootElement.TryGetProperty("Error", out JsonElement errorProperty);
            Error error = JsonSerializer.Deserialize<Error>(errorProperty)!;
            return error;
        }
        catch
        {
            return new Error("Error in deserialization happened.");
        }
    }
}
