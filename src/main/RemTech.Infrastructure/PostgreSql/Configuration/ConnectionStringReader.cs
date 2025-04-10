using System.Text.Json;

namespace RemTech.Infrastructure.PostgreSql.Configuration;

/// <summary>
/// Класс для чтения строки подключения к БД из .json файла.
/// </summary>
public static class ConnectionStringReader
{
    /// <summary>
    /// Чтение строки подключения из .json файла.
    /// </summary>
    /// <param name="filePath">Путь к .json файлу со строкой подключения.</param>
    /// <returns>Connection String объект, содержащий значение строки подключения.</returns>
    /// <exception cref="ArgumentException">Исключение при проблемах с файлом.</exception>
    /// <exception cref="InvalidDataException">Исключение при проблемах с парсингом файла.</exception>
    public static ConnectionString CreateFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("Путь к файлу со строкой подключению был пустым.");

        if (!File.Exists(filePath))
            throw new ArgumentException("Файл со строкой подключения не был найден.");

        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(filePath));

        if (!document.RootElement.TryGetProperty("Host", out JsonElement host))
            throw new InvalidDataException($"Host в файле: {filePath} не найден.");

        if (!document.RootElement.TryGetProperty("Port", out JsonElement port))
            throw new InvalidDataException($"Port в файле: {filePath} не найден.");

        if (!document.RootElement.TryGetProperty("Username", out JsonElement userName))
            throw new InvalidDataException($"Username в файле: {filePath} не найден.");

        if (!document.RootElement.TryGetProperty("Password", out JsonElement password))
            throw new InvalidDataException($"Password в файле: {filePath} не найден.");

        if (!document.RootElement.TryGetProperty("Database", out JsonElement database))
            throw new InvalidDataException($"Database в файле: {filePath} не найден.");

        string? hostValue = host.GetString();
        if (string.IsNullOrWhiteSpace(hostValue))
            throw new InvalidDataException($"Host в файле: {filePath} был пустым.");

        string? portValue = port.GetString();
        if (string.IsNullOrWhiteSpace(portValue))
            throw new InvalidDataException($"Port в файле: {filePath} был пустым.");

        string? userNameValue = userName.GetString();
        if (string.IsNullOrWhiteSpace(userNameValue))
            throw new InvalidDataException($"Username в файле: {filePath} был пустым.");

        string? passwordValue = password.GetString();
        if (string.IsNullOrWhiteSpace(passwordValue))
            throw new InvalidDataException($"Password в файле: {filePath} был пустым.");

        string? databaseValue = database.GetString();
        if (string.IsNullOrWhiteSpace(databaseValue))
            throw new InvalidDataException($"Database в файле: {filePath} не найден.");

        string connectionString =
            $"Host={hostValue}; Port={portValue}; Username={userNameValue}; Password={passwordValue}; Database={databaseValue}";

        return new ConnectionString(connectionString);
    }
}
