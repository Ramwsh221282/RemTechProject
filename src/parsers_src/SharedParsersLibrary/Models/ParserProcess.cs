using RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;
using RemTech.Infrastructure.PostgreSql.ParserContext.DaoModels;
using RemTech.Shared.SDK.OptionPattern;
using Serilog;
using SharedParsersLibrary.Contracts;
using SharedParsersLibrary.DatabaseSinking;

namespace SharedParsersLibrary.Models;

public sealed class ParserProcess(
    ILogger logger,
    string parserName,
    ParserManagementFacade parserFacade,
    IScrapeAdvertisementHandlerFactory factory
)
{
    private readonly ILogger _logger = logger;
    private readonly ParserManagementFacade _parserFacade = parserFacade;
    private readonly IScrapeAdvertisementHandlerFactory _factory = factory;
    private readonly string _parserName = parserName;

    public async Task Invoke(CancellationToken ct = default)
    {
        _logger.Information("{Context} requesting parser configuration...", nameof(ParserProcess));
        Option<ParserDao> parserOption = await _parserFacade.GetParser(_parserName);
        if (parserOption.HasValue == false)
        {
            _logger.Warning(
                "{Context} parser context {Name} was not found.",
                nameof(ParserProcess),
                _parserName
            );
            return;
        }

        ParserDao response = parserOption.Value;
        List<ParserProfileDao> profiles = response.Profiles;
        foreach (ParserProfileDao profile in profiles)
        {
            if (!IsProfileNextRunDateNow(profile))
            {
                _logger.Warning(
                    "{Context} profile {Name} time to run is not now yet.",
                    nameof(ParserProcess),
                    profile.Name
                );
                continue;
            }

            string[] links = profile.GetDeserializedLinksArray();

            if (links.Length == 0)
            {
                _logger.Warning(
                    "{Context} profile {Name} has no links set.",
                    nameof(ParserProcess),
                    profile.Name
                );
                continue;
            }

            if (IsProfileDisabled(profile))
            {
                _logger.Warning(
                    "{Context} profile {Name} is disabled.",
                    nameof(ParserProcess),
                    profile.Name
                );
                continue;
            }

            await ProcessProfile(profile, links);
        }
    }

    private static bool IsProfileNextRunDateNow(ParserProfileDao profile)
    {
        long seconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        return seconds >= profile.NextRunUnixSeconds;
    }

    private static bool IsProfileDisabled(ParserProfileDao profile)
    {
        return profile.State == ParserProfileState.Disabled.State;
    }

    private async Task ProcessProfile(ParserProfileDao profile, string[] links)
    {
        foreach (string link in links)
        {
            ScrapeAdvertisementsCommand command = new(link);
            IScrapeAdvertisementsHandler handler = _factory.Create();
            await _parserFacade.MakeWorking(profile);

            try
            {
                await handler.Handle(command);
                _logger.Information(
                    "{Context} scraped link: {Link}. Success",
                    nameof(ParserProcess),
                    link
                );
                await _parserFacade.MakeSleeping(profile);
            }
            catch (Exception ex)
            {
                _logger.Fatal(
                    "{Context} scraped link: {Link}. Exception: {Ex}",
                    nameof(ParserProcess),
                    link,
                    ex.Message
                );
                await _parserFacade.MakeSleeping(profile);
            }
        }
    }
}
