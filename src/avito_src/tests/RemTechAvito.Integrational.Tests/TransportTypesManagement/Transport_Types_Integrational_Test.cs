using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Application.Abstractions.Handlers;
using RemTechAvito.Application.TransportAdvertisementsManagement.TransportAdvertisements.Commands.CreateCustomTransportType;
using RemTechAvito.DependencyInjection;
using Serilog;

namespace RemTechAvito.Integrational.Tests.TransportTypesManagement;

public class Transport_Types_Integrational_Test
{
    private readonly IServiceProvider _provider;
    private readonly ILogger _logger;

    public Transport_Types_Integrational_Test()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        _provider = services.BuildServiceProvider();
        _logger = _provider.GetRequiredService<ILogger>();
    }

    [Fact]
    public async Task Create_User_Transport_Type()
    {
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var noExceptions = true;
        try
        {
            var command = new CreateCustomTransportTypeCommand(
                "My Custom",
                "http://localhost.com",
                []
            );
            var handler = _provider.GetRequiredService<
                IAvitoCommandHandler<CreateCustomTransportTypeCommand>
            >();
            var handle = await handler.Handle(command, ct);
            Assert.True(handle.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Create_User_Transport_Type),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
