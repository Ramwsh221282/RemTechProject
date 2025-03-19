namespace SharedParsersLibrary.Contracts;

public interface IScrapeAdvertisementsHandler
{
    public Task Handle(ScrapeAdvertisementCommand command);
}
