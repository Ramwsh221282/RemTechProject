using System.Diagnostics;
using Microsoft.Extensions.DependencyInjection;
using RemTechAvito.Contracts.Common.Dto.ParserJournalsManagement;
using RemTechAvito.Core.Common.ValueObjects;
using RemTechAvito.Core.ParserJournalManagement;
using RemTechAvito.DependencyInjection;
using RemTechAvito.Infrastructure.Contracts.Repository;

namespace RemTechAvito.Integrational.Tests.ParserJournalManagementTests;

public sealed class Parser_Journal_CRUD_Tests
{
    private readonly IServiceProvider _provider;

    public Parser_Journal_CRUD_Tests()
    {
        IServiceCollection services = new ServiceCollection();
        services.RegisterServices();
        _provider = services.BuildServiceProvider();
    }

    [Fact]
    public async Task Create_Parser_Journal()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
            sw.Stop();
            var journal = ParserJournal.CreateSuccess(
                Time.Create(sw.Elapsed),
                10,
                "http://localhost:228",
                "Test case"
            );
            var repository = _provider.GetRequiredService<IParserJournalCommandRepository>();
            var insert = await repository.Add(journal, ct);
            Assert.True(insert.IsSuccess);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }

    [Fact]
    public async Task Create_And_Receive_Journal()
    {
        var noExceptions = true;
        using var cts = new CancellationTokenSource();
        var ct = cts.Token;
        try
        {
            var sw = new Stopwatch();
            sw.Start();
            await Task.Delay(TimeSpan.FromSeconds(5), cts.Token);
            sw.Stop();
            var journal = ParserJournal.CreateSuccess(
                Time.Create(sw.Elapsed),
                10,
                "http://localhost:228",
                "Test case"
            );
            var repository = _provider.GetRequiredService<IParserJournalCommandRepository>();
            var insert = await repository.Add(journal, ct);
            Assert.True(insert.IsSuccess);
            var queryRepository = _provider.GetRequiredService<IParserJournalQueryRepository>();
            var items = await queryRepository.Get(1, 1, ct);
            Assert.NotEmpty(items);
        }
        catch
        {
            noExceptions = false;
        }

        Assert.True(noExceptions);
    }
}
