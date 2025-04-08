using RemTech.Domain.ParserContext.Entities.ParserProfiles;
using RemTech.Domain.ParserContext.ValueObjects;
using RemTech.Shared.SDK.OptionPattern;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext;

public sealed class Parser
{
    private readonly List<ParserProfile> _profiles = [];
    public ParserId Id { get; }
    public ParserName Name { get; } = null!;
    public IReadOnlyCollection<ParserProfile> Profiles => _profiles;

    private Parser() { } // ef core constructor.

    public Parser(ParserId id, ParserName name)
    {
        Id = id;
        Name = name;
        _profiles = [];
    }

    /// <summary>
    /// Добавление нового уникального профиля в парсер.
    /// </summary>
    /// <param name="profile">Профиль.</param>
    /// <returns>Результат добавления. Success если был добавлен. Failure если не был добавлен.</returns>
    public Result AddProfile(ParserProfile profile)
    {
        if (profile.Parser.Id != Id)
            return new Error($"Добавляемый профиль не принадлежит парсеру {Name}.");

        if (!IsProfileUnique(profile))
            return new Error($"Добавляемый профиль {profile.Name.Value} не уникален.");

        _profiles.Add(profile);
        return Result.Success();
    }

    /// <summary>
    /// Удаление профиля по наименованию.
    /// </summary>
    /// <param name="name">Название профиля.</param>
    /// <returns>Результат удаления. Guid если был найден и удалён. Failure если не был найден.</returns>
    public Result<Guid> RemoveProfile(string name)
    {
        Option<ParserProfile> profile = GetProfileByName(name);
        if (!profile.HasValue)
            return new Error($"Профиль с именем {name} не найден в парсере {Name}.");

        _profiles.Remove(profile.Value);
        return profile.Value.Id.Value;
    }

    /// <summary>
    /// Получение профиля по имени в текущем Parser.
    /// </summary>
    /// <param name="name">Название профиля.</param>
    /// <returns>Option объект. Если был найден то с HasValue = true, если не был найден, то с HasValue = false.</returns>
    public Option<ParserProfile> GetProfileByName(string name)
    {
        ParserProfile? profile = _profiles.FirstOrDefault(p => p.Name.Value == name);
        return profile == null ? Option<ParserProfile>.None() : Option<ParserProfile>.Some(profile);
    }

    /// <summary>
    /// Проверка на уникальность профиля в текущем Parser.
    /// </summary>
    /// <param name="profile">Профиль.</param>
    /// <returns>True если профиль уникален. False если профиль с таким именем уже есть в Parser.</returns>
    private bool IsProfileUnique(ParserProfile profile) =>
        _profiles.All(p => p.Name != profile.Name);
}
