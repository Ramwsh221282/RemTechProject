using Microsoft.Extensions.DependencyInjection;
using RemTech.Infrastructure.PostgreSql.ParserContext.Queries.Responses.ResponseModels;
using RemTech.Shared.SDK.ResultPattern;
using Serilog;
using SharedParsersLibrary.Contracts;

namespace SharedParsersLibrary.Models;

public sealed class ParserProcess(ILogger logger, IServiceProvider provider)
{
    private readonly ILogger _logger = logger;
    private readonly IServiceProvider _provider = provider;
    public string ParserName { get; set; } = string.Empty;

    public async Task Invoke(CancellationToken ct = default)
    {
        _logger.Information("{Context} requesting parser configuration...", nameof(ParserProcess));
        // TODO Request parser configuration.
    }

    private async Task InvokeScraping(ParserResponse parser)
    {
        string[] links = [.. parser.Profiles.SelectMany(p => p.Links)];

        foreach (string link in links)
        {
            ScrapeAdvertisementsCommand command = new(link);

            IScrapeAdvertisementsHandler handler =
                _provider.GetRequiredService<IScrapeAdvertisementsHandler>();

            await handler.Handle(command);
        }
    }

    private async Task SleepForMinute()
    {
        // TODO perform SQL query to request a parser with given ParserName.
    }

    private async Task<Result<ParserResponse>> RequestParser()
    {
        // TODO perform SQL query to request a parser with given ParserName.
        throw new NotImplementedException();
    }
}
