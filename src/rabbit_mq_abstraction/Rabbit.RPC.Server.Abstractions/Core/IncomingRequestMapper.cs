using System.Text.Json;
using Rabbit.RPC.Server.Abstractions.Communication;
using Serilog;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class IncomingRequestMapper
{
    private readonly Dictionary<string, Type> _contractTypes;

    private readonly ILogger _logger;

    public IncomingRequestMapper(Dictionary<string, Type> types, ILogger logger)
    {
        _contractTypes = types;
        _logger = logger;
    }

    public IContract? MapToContract(string json)
    {
        _logger.Information("Received json: {Json} for mapping", json);
        using JsonDocument document = JsonDocument.Parse(json);

        if (
            !document.RootElement.TryGetProperty(
                "OperationName",
                out JsonElement operationNameProperty
            )
        )
        {
            _logger.Error("Json {Json} is not of supported service operation type", json);
            return null;
        }

        string? operationNameValue = operationNameProperty.GetString();
        if (string.IsNullOrEmpty(operationNameValue))
        {
            _logger.Error("Json {Json} has no OperationName property value", json);
            return null;
        }

        if (!_contractTypes.TryGetValue(operationNameValue, out Type? requestType))
        {
            _logger.Error("Json {Json} is not of supported service operation type", json);
            return null;
        }

        IContract? contract = JsonSerializer.Deserialize(json, requestType) as IContract;

        _logger.Information(
            "Deserialized json: {Json} into IContract of type: {Type}",
            json,
            contract?.GetType().Name
        );

        return contract;
    }
}
