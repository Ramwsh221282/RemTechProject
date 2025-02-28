using RemTechCommon.Utils.Extensions;
using RemTechCommon.Utils.MonadPattern;
using RemTechCommon.Utils.ResultPattern;

namespace RemTechAvito.Core.ParserProfileManagement.ValueObjects;

public record ParserProfileLink
{
    public string Name { get; protected init; }
    public string Link { get; protected init; }

    protected ParserProfileLink(string name, string link)
    {
        Name = name;
        Link = link;
    }

    public Result<T> Unwrap<T>()
        where T : ParserProfileLink
    {
        return this is T ? (T)this : new Error($"Cannot unwrap to {typeof(T).Name}");
    }

    internal static Result<ParserProfileLink> Create(string name, string link)
    {
        var result = ValidatedFactoryMonad<string>
            .Start(name)
            .Set(string.IsNullOrWhiteSpace, link, "Parser profile name is empty")
            .Set(l => !UrlLinkValidator.IsStringUrl(l), "Parser link is not valid")
            .Factory(() => new ParserProfileLink(name, link));
        return result;
    }
}

public sealed record ParserProfileLinkWithAdditions : ParserProfileLink
{
    private readonly List<string> _additions = [];

    public IReadOnlyCollection<string> Additions
    {
        get => _additions;
        private init
        {
            _additions.Clear();
            _additions.AddRange(value);
        }
    }

    private ParserProfileLinkWithAdditions(string name, string link, List<string> additions)
        : base(name, link)
    {
        _additions = additions;
    }

    internal static Result<ParserProfileLink> Create(
        string name,
        string link,
        List<string> additions
    )
    {
        var result = ValidatedFactoryMonad<string>
            .Start(name)
            .Set(string.IsNullOrWhiteSpace, link, "Parser profile name is be empty")
            .Set(l => !UrlLinkValidator.IsStringUrl(l), additions, "Parser profile is not valid")
            .Set(
                adds => adds.Any(string.IsNullOrWhiteSpace),
                "Parser profile additions are not valid"
            )
            .Factory(() => new ParserProfileLinkWithAdditions(name, link, additions));
        return result.IsFailure ? result.Error : result.Value;
    }

    public ParserProfileLink Update(string name, string link, IEnumerable<string> additions)
    {
        return this with { Name = name, Link = link, Additions = additions.ToList() };
    }
}

public static class ParserProfileLinkFactory
{
    public static Result<ParserProfileLink> Create(
        string name,
        string link,
        List<string>? additions = null
    )
    {
        return additions switch
        {
            null => ParserProfileLink.Create(name, link),
            not null when additions.Count == 0 => ParserProfileLink.Create(name, link),
            not null when additions.Count > 0 => ParserProfileLinkWithAdditions.Create(
                name,
                link,
                additions
            ),
            _ => new Error("Unsupported parser profile links structure"),
        };
    }
}
