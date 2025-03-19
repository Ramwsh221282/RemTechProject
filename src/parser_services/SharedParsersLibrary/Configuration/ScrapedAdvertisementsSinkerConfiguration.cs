namespace SharedParsersLibrary.Configuration;

public sealed class ScrapedAdvertisementsSinkerConfiguration
{
    public string QueueName { get; }
    public string HostName { get; }
    public string Username { get; }
    public string Password { get; }

    public ScrapedAdvertisementsSinkerConfiguration(
        string queueName,
        string hostName,
        string username,
        string password
    )
    {
        QueueName = queueName;
        HostName = hostName;
        Username = username;
        Password = password;
    }
}
