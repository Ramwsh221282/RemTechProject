using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Contracts.Common.Dto.TransportAdvertisementsManagement;
using RemTechAvito.Core.AdvertisementManagement.TransportAdvertisement;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;
using Serilog;

namespace RemTechAvito.MongoDb.Tests.BasicCrudTests;

public sealed class Reader_Entities_Tests
{
    private readonly ILogger _logger;
    private readonly ITransportAdvertisementsQueryRepository _repository;

    public Reader_Entities_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        IServiceProvider provider = services.BuildServiceProvider();
        _logger = provider.GetRequiredService<ILogger>();
        _repository = provider.GetRequiredService<ITransportAdvertisementsQueryRepository>();
    }

    [Fact]
    public async Task Read_With_Characteristics_Set_Test()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            CharacteristicsListDto listDto = new CharacteristicsListDto(
                [new CharacteristicDto("Марка", "Kandela")]
            );

            FilterAdvertisementsDto dto = new FilterAdvertisementsDto(Characteristics: listDto);
            IEnumerable<TransportAdvertisement> data = await _repository.Query(dto);

            listDto = new CharacteristicsListDto(
                [new CharacteristicDto("Другой Текст", "Другой текст")]
            );
            dto = new FilterAdvertisementsDto(Characteristics: listDto);
            data = await _repository.Query(dto);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Error: {Ex}",
                nameof(Read_With_Characteristics_Set_Test),
                ex.Message
            );
        }
        Assert.True(noExceptions);
    }
}
