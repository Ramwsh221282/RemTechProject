using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;

namespace RemTechAvito.Integrational.Tests.TransportTypesManagement;

public sealed class Transport_Types_CRUD_Tests
{
    private readonly IServiceProvider _provider;

    public Transport_Types_CRUD_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Get_All_Transport_Types()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            var repository = _provider.GetRequiredService<ITransportTypesQueryRepository>();
            var items = await repository.Get(token);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Paged_Transport_Types()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            var pagination = new GetTransportTypesQueryPagination(1, 50);
            var query = new GetTransportTypesQuery(pagination);
            var repository = _provider.GetRequiredService<ITransportTypesQueryRepository>();
            var items = await repository.Get(query, token);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Sorted_Ascending_Transport_Types()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            var sorting = new GetTransportTypesQuerySortMode("ASC");
            var pagination = new GetTransportTypesQueryPagination(1, 50);
            var query = new GetTransportTypesQuery(pagination, sorting);
            var repository = _provider.GetRequiredService<ITransportTypesQueryRepository>();
            var items = await repository.Get(query, token);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_Text_Search_Transport_Types()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var token = cts.Token;

        try
        {
            var sorting = new GetTransportTypesQuerySortMode("ASC");
            var textSearch = new GetTransportTypesTextSearch("lugong");
            var pagination = new GetTransportTypesQueryPagination(1, 50);
            var query = new GetTransportTypesQuery(pagination, sorting, textSearch);
            var repository = _provider.GetRequiredService<ITransportTypesQueryRepository>();
            var items = await repository.Get(query, token);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }
}
