using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.FiltersManagement.TransportTypes;

public abstract record TransportType(string Type, string Name, string Link, DateOnly CreatedOn)
{
    public const string SYSTEM_TYPE = "SYSTEM";
    public const string USER_TYPE = "USER";

    public Result<T> Unwrap<T>()
        where T : TransportType
    {
        return this is T ? (T)this : new Error($"Cannot unwrap to {typeof(T).Name}");
    }

    protected static Result<string> ValidatedUrlLink(string input)
    {
        var isValid = UrlLinkValidator.IsStringUrl(input);
        return isValid ? input : new Error("Link is not valid.");
    }

    protected static Result<string> ValidatedName(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new Error("Name is required.");
        return input;
    }
}

public sealed record UserTransportType : TransportType
{
    private readonly List<string> _additions;

    private readonly List<string> _profiles = [];

    public IReadOnlyCollection<string> Profiles
    {
        // bson class map
        get => _profiles;
        private set
        {
            _profiles.Clear();
            _profiles.AddRange(value);
        }
    }

    public IReadOnlyCollection<string> Additions
    {
        // bson class map
        get => _additions;
        private set
        {
            _additions.Clear();
            _additions.AddRange(value);
        }
    }

    private UserTransportType(
        string name,
        string link,
        DateOnly createdOn,
        List<string>? textSearchAdditions = null
    )
        : base(USER_TYPE, name, link, createdOn)
    {
        _additions = textSearchAdditions ?? [];
    }

    public static Result<TransportType> Create(
        string name,
        string link,
        DateOnly createdOn,
        List<string>? textSearchAdditions = null
    )
    {
        var validatedLink = ValidatedUrlLink(link);
        if (validatedLink.IsFailure)
            return validatedLink.Error;
        var validatedName = ValidatedName(name);
        if (validatedName.IsFailure)
            return validatedName.Error;
        return new UserTransportType(
            validatedName.Value,
            validatedLink.Value,
            createdOn,
            textSearchAdditions
        );
    }

    public UserTransportType Update(List<string>? profiles = null, List<string>? additions = null)
    {
        return new UserTransportType(Name, Link, CreatedOn)
        {
            Additions = additions ?? _additions,
            Profiles = profiles ?? _profiles,
        };
    }
}

public sealed record SystemTransportType : TransportType
{
    private SystemTransportType(string name, string link, DateOnly createdOn)
        : base(SYSTEM_TYPE, name, link, createdOn) { }

    public static Result<TransportType> Create(string name, string link, DateOnly createdOn)
    {
        var validatedLink = ValidatedUrlLink(link);
        if (validatedLink.IsFailure)
            return validatedLink.Error;
        var validatedName = ValidatedName(name);
        if (validatedName.IsFailure)
            return validatedName.Error;
        return new SystemTransportType(validatedName.Value, validatedLink.Value, createdOn);
    }
}
