namespace AvitoParser.PDK;

public sealed record QuerySelectorPayload
{
    public string Selector { get; }

    public QuerySelectorPayload(string selector)
    {
        Span<char> span = new Span<char>(new char[selector.Length + 1]) { [0] = '.' };
        int spanLastIndex = 1;

        foreach (char character in selector)
        {
            if (character == ' ')
            {
                span[spanLastIndex] = '.';
                spanLastIndex++;
                continue;
            }

            span[spanLastIndex] = character;
            spanLastIndex++;
        }

        Selector = new string(span);
    }
}
