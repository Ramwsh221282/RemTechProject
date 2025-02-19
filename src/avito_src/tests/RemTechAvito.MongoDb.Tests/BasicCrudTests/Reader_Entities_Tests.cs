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
    public async Task Test_Get_Analytics_Items_With_Filter_Sample_1()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            PriceRangeDto dto = new PriceRangeDto(3000000, 6000000);
            FilterAdvertisementsDto filter = new FilterAdvertisementsDto(PriceRange: dto);
            GetAnalyticsStatsRequest request = new GetAnalyticsStatsRequest(filter);
            AnalyticsStatsResponse response = await _repository.Query(request, ct);
            _logger.Information("Analytics: {Analytics}", response);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Test_Get_Analytics_Items_With_Filter_Sample_1),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Test_Get_Analytics_Items_Without_Filter_Sample_1()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            GetAnalyticsStatsRequest request = new GetAnalyticsStatsRequest();
            AnalyticsStatsResponse response = await _repository.Query(request, ct);
            _logger.Information("Analytics: {Analytics}", response);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Test_Get_Analytics_Items_Without_Filter_Sample_1),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Test_Get_Items_With_Filter_Sample_1()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            PriceRangeDto dto = new PriceRangeDto(3000000, 6000000);
            FilterAdvertisementsDto filter = new FilterAdvertisementsDto(PriceRange: dto);
            GetAnalyticsItemsRequest request = new GetAnalyticsItemsRequest(1, 5, filter);
            AdvertisementItemResponse[] response = await _repository.Query(request, ct);
            _logger.Information("Count: {Count}", response.Length);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Test_Get_Items_With_Filter_Sample_1),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Test_Get_Items_Without_Filter_Sample_1()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            GetAnalyticsItemsRequest request = new GetAnalyticsItemsRequest(1, 5);
            AdvertisementItemResponse[] response = await _repository.Query(request, ct);
            _logger.Information("Count: {Count}", response.Length);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Test_Get_Items_Without_Filter_Sample_1),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Test_Get_Items_With_Filter_Sample_1_Asc()
    {
        using CancellationTokenSource cts = new CancellationTokenSource();
        CancellationToken ct = cts.Token;
        bool noExceptions = true;

        try
        {
            PriceRangeDto dto = new PriceRangeDto(3000000, 6000000);
            FilterAdvertisementsDto filter = new FilterAdvertisementsDto(PriceRange: dto);
            GetAnalyticsItemsRequest request = new GetAnalyticsItemsRequest(
                1,
                5,
                filter,
                SortOrder: "ASC"
            );

            AdvertisementItemResponse[] response = await _repository.Query(request, ct);
            _logger.Information("Count: {Count}", response.Length);
            foreach (var item in response)
            {
                _logger.Information("{Price}", item.Price.Value);
            }
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Exception: {Ex}",
                nameof(Test_Get_Items_With_Filter_Sample_1),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
