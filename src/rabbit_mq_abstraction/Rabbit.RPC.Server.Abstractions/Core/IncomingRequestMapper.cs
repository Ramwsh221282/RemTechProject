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

    public ContractMappingResult MapToContract(string json)
    {
        try
        {
            using JsonDocument doc = JsonDocument.Parse(json);
            if (!doc.RootElement.TryGetProperty("Name", out JsonElement contractNameProperty))
            {
                _logger.Error("Json {Json} hasn't contract name.", json);
                return new($"Json {json} hasn't contract name.");
            }

            string? contractNameValue = contractNameProperty.GetString();
            if (string.IsNullOrEmpty(contractNameValue))
            {
                _logger.Error("Json {Json} contract name is empty.", json);
                return new($"Json {json} contract name is empty.");
            }

            if (!_contractTypes.TryGetValue(contractNameValue, out Type? requestType))
            {
                _logger.Error("Json {Json} is not supported in this context.", json);
                return new($"Json {json} is not supported in this context.");
            }

            doc.RootElement.TryGetProperty("Body", out JsonElement bodyProperty);
            if (JsonSerializer.Deserialize(bodyProperty, requestType) is not IContract contract)
            {
                _logger.Error("Json {Json} is not supported in this context.", json);
                return new($"Json {json} is not supported in this context.");
            }

            _logger.Information("Received contract {Type}", contract.GetType().Name);
            return new(contract);
        }
        catch (Exception ex)
        {
            string message = ex.Message;
            _logger.Error("Service exception: {Message}", message.AsMemory());
            return new ContractMappingResult(message);
        }
    }
}

public sealed class ContractMappingResult
{
    public bool IsSuccess { get; }
    public string Error { get; }
    public IContract Contract { get; } = null!;

    public ContractMappingResult(string error)
    {
        Error = error;
    }

    public ContractMappingResult(IContract contract)
    {
        Error = string.Empty;
        Contract = contract;
        IsSuccess = true;
    }
}
