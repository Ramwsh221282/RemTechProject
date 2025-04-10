using System.Collections;
using RemTech.Shared.SDK.ResultPattern;

namespace RemTech.Domain.ParserContext.Entities.ParserProfiles.ValueObjects;

public sealed record ParserProfileLink
{
    public string Link { get; }

    private ParserProfileLink(string link) => Link = link;

    public static Result<ParserProfileLink> Create(string link)
    {
        if (string.IsNullOrWhiteSpace(link))
            return new Error("Ссылка для парсинга была пустой.");
        return new ParserProfileLink(link);
    }
}

public sealed record ParserProfileLinksCollection : IReadOnlyList<ParserProfileLink>
{
    private readonly List<ParserProfileLink> _links = [];
    public IReadOnlyList<ParserProfileLink> Links => _links;

    private ParserProfileLinksCollection() { } // ef core

    private ParserProfileLinksCollection(ParserProfileLink[] links) => _links = [.. links];

    public static Result<ParserProfileLinksCollection> Create(ParserProfileLink[] links)
    {
        HashSet<string> uniqueValues = [];
        foreach (ParserProfileLink link in links)
        {
            if (!uniqueValues.Add(link.Link))
                return new Error($"Ссылка: {link.Link} не уникальна.");
        }

        return new ParserProfileLinksCollection(links);
    }

    public IEnumerator<ParserProfileLink> GetEnumerator()
    {
        return Links.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => Links.Count;

    public ParserProfileLink this[int index] => Links[index];

    public static ParserProfileLinksCollection Empty() => new(Array.Empty<ParserProfileLink>());
}
