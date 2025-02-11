using Rabbit.RPC.Server.Abstractions.Communication;

namespace Rabbit.RPC.Server.Abstractions.Core;

public sealed class SimpleServerProcess : IServerProcess
{
    private readonly IncomingRequestMapper _mapper;
    private readonly ContractsResolvingCenter _center;

    public SimpleServerProcess(IncomingRequestMapper mapper, ContractsResolvingCenter center)
    {
        _mapper = mapper;
        _center = center;
    }

    public async Task<ContractActionResult> HandleMessage(string receivedJson)
    {
        try
        {
            ContractMappingResult mapping = _mapper.MapToContract(receivedJson);
            if (!mapping.IsSuccess)
                return ContractActionResult.Fail(mapping.Error);

            ContractActionResult response = await _center.ResolveContract(mapping.Contract);
            return response;
        }
        catch (Exception ex)
        {
            return ContractActionResult.Fail(ex.Message);
        }
    }
}
