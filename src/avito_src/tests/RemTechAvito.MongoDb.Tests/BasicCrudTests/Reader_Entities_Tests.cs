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
                [new CharacteristicDto("Марка", "Merlo")]
            );
            FilterAdvertisementsDto dto = new FilterAdvertisementsDto(
                CharacteristicsSearch: listDto
            );
            IEnumerable<TransportAdvertisement> data = await _repository.Query(dto);
            foreach (var item in data)
            {
                bool containsMark = item.Characteristics.Data.Any(d =>
                    d.Name.Contains("Марка") && d.Value.Contains("Merlo")
                );
                Assert.True(containsMark);
            }

            listDto = new CharacteristicsListDto(
                [new CharacteristicDto("Другой Текст", "Другой текст")]
            );

            dto = new FilterAdvertisementsDto(CharacteristicsSearch: listDto);
            data = await _repository.Query(dto);
            Assert.Empty(data);

            listDto = new CharacteristicsListDto([new CharacteristicDto("Модель", "P 50.18 HM")]);
            dto = new FilterAdvertisementsDto(CharacteristicsSearch: listDto);
            data = await _repository.Query(dto);

            listDto = new CharacteristicsListDto(
                [new CharacteristicDto("Тип техники", "Телескопический погрузчик")]
            );
            dto = new FilterAdvertisementsDto(CharacteristicsSearch: listDto);
            data = await _repository.Query(dto);

            listDto = new CharacteristicsListDto([new CharacteristicDto("", "2008")]);
            dto = new FilterAdvertisementsDto(CharacteristicsSearch: listDto);
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
