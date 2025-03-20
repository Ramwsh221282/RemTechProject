namespace SharedParsersLibrary.Puppeteer.Features.PageBehavior;

public sealed class ClassNameFormatter
{
    private readonly string _className;

    public ClassNameFormatter(string className)
    {
        _className = className;
    }

    public string MakeFormatted()
    {
        Span<char> span = new Span<char>(new char[_className.Length + 1]) { [0] = '.' };
        int spanLastIndex = 1;

        foreach (char character in _className)
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

        return new string(span);
    }
}
