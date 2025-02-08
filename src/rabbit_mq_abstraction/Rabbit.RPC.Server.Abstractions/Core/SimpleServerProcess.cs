using Rabbit.RPC.Server.Abstractions.Communication;
using Serilog;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class SimpleServerProcess : IServerProcess
{
    private readonly IncomingRequestMapper _mapper;
    private readonly ContractsResolvingCenter _center;
    private readonly ILogger _logger;

    public SimpleServerProcess(
        IncomingRequestMapper mapper,
        ContractsResolvingCenter center,
        ILogger logger
    )
    {
        _mapper = mapper;
        _center = center;
        _logger = logger;
    }

    public async Task<string> HandleMessage(string receivedJson)
    {
        IContract? contract = _mapper.MapToContract(receivedJson);
        if (contract == null)
        {
            _logger.Error("No contract allowed for: {Json}", receivedJson);
            return "{}";
        }

        string responseJson = await _center.ResolveContract(contract);

        _logger.Information(
            "Resolved request: {Request} and created response: {Response}",
            receivedJson,
            responseJson
        );

        return responseJson;
    }
}
