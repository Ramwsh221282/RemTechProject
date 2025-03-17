using Rabbit.RPC.Client.Abstractions;
using RemTech.MainApi.ParsersManagement.Configurations;

namespace RemTech.MainApi.ParsersManagement.Messages;

public sealed class DataServiceMessagerFactory
{
    private readonly ParserDataServiceConfiguration _configuration;

    public DataServiceMessagerFactory(ParserDataServiceConfiguration configuration) =>
        _configuration = configuration;

    public DataServiceMessager Create() =>
        new DataServiceMessager(CreateSingleCommunicationPublisher());

    private SingleCommunicationPublisher CreateSingleCommunicationPublisher() =>
        new SingleCommunicationPublisher(
            _configuration.QueueName,
            _configuration.HostName,
            _configuration.UserName,
            _configuration.Password
        );
}
