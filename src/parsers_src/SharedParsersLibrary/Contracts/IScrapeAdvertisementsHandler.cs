namespace SharedParsersLibrary.Contracts;

public interface IScrapeAdvertisementsHandler
{
    Task Handle(ScrapeAdvertisementsCommand command);
}
