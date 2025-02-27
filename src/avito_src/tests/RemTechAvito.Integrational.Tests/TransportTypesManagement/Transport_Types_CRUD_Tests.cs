using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Contracts.Common.Dto.TransportTypesManagement;
using RemTechAvito.Contracts.Common.Responses.TransportTypesManagement;
using RemTechAvito.Core.FiltersManagement.TransportTypes;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;
using RemTechCommon.Utils.ResultPattern;
using Serilog;

namespace RemTechAvito.Integrational.Tests.TransportTypesManagement;

public sealed class Transport_Types_CRUD_Tests
{
    private readonly IServiceProvider _provider;
    private readonly ILogger _logger;

    public Transport_Types_CRUD_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        _provider = services.BuildServiceProvider();
        _logger = _provider.GetRequiredService<ILogger>();
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
            var implementor = new GetTransportTypesImplementor(TransportType.SYSTEM_TYPE);
            var query = new GetTransportTypesQuery(pagination, implementor);
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
            var implementor = new GetTransportTypesImplementor(TransportType.SYSTEM_TYPE);
            var query = new GetTransportTypesQuery(pagination, implementor);
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
            var implementor = new GetTransportTypesImplementor(TransportType.SYSTEM_TYPE);
            var query = new GetTransportTypesQuery(pagination, implementor, sorting, textSearch);
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
    public async Task Create_And_Insert_User_Custom_Transport_Type()
    {
        var noExceptions = true;
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try
        {
            var createdOn = DateOnly.FromDateTime(DateTime.Now);
            var type = UserTransportType.Create(
                "Liugong 855H",
                "https://localhost.com",
                createdOn,
                ["855H"]
            );
            Assert.True(type.IsSuccess);
            var repository = _provider.GetRequiredService<ITransportTypesCommandRepository>();
            var insertion = await repository.Add(type, ct);
            Assert.True(insertion.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Error: {Exception}",
                nameof(Create_And_Insert_User_Custom_Transport_Type),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Create_And_Insert_User_Custom_Transport_Type_With_Profile()
    {
        var noExceptions = true;
        var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try
        {
            var createdOn = DateOnly.FromDateTime(DateTime.Now);
            var type = UserTransportType.Create(
                "Liugong 855H",
                "https://localhost.com",
                createdOn,
                ["855H"]
            );

            UserTransportType userType = type.Value.Unwrap<UserTransportType>();
            userType = userType.Update(["Liugong 855H"], userType.Additions.ToList());
            type = userType;
            var repository = _provider.GetRequiredService<ITransportTypesCommandRepository>();
            var insertion = await repository.Add(type, ct);
            Assert.True(insertion.IsSuccess);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Error: {Exception}",
                nameof(Create_And_Insert_User_Custom_Transport_Type_With_Profile),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Get_User_Transport_Types()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        var token = cts.Token;
        try
        {
            var createdOn = DateOnly.FromDateTime(DateTime.Now);
            var type = UserTransportType.Create(
                "Liugong 855H",
                "https://localhost.com",
                createdOn,
                ["855H"]
            );

            UserTransportType userType = type.Value.Unwrap<UserTransportType>();
            userType = userType.Update(["Liugong 855H"], userType.Additions.ToList());
            type = userType;
            var repository = _provider.GetRequiredService<ITransportTypesCommandRepository>();
            var insertion = await repository.Add(type, ct);
            var readRepository = _provider.GetRequiredService<ITransportTypesQueryRepository>();
            var pagination = new GetTransportTypesQueryPagination(1, 10);
            var implementor = new GetTransportTypesImplementor(TransportType.USER_TYPE);
            var query = new GetTransportTypesQuery(pagination, implementor);
            var response = await readRepository.Get(query, ct);
        }
        catch (Exception ex)
        {
            noExceptions = false;
            _logger.Fatal(
                "{Test} failed. Error: {ex}",
                nameof(Get_User_Transport_Types),
                ex.Message
            );
        }

        Assert.True(noExceptions);
    }
}
