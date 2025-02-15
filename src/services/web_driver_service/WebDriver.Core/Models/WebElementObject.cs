using System.Buffers;
using System.IO.Compression;
using System.Text;
using OpenQA.Selenium;

namespace WebDriver.Core.Models;

public record WebElementObject : IDisposable
{
    private IMemoryOwner<byte>? _outerHtmlBytesOwner;
    private IMemoryOwner<byte>? _innerTextBytesOwner;
    public IWebElement Model { get; }
    public Guid ElementId { get; }

    public ReadOnlyMemory<byte> ElementOuterHTMLBytes =>
        _outerHtmlBytesOwner?.Memory ?? ReadOnlyMemory<byte>.Empty;
    public ReadOnlyMemory<byte> ElementInnerTextBytes =>
        _innerTextBytesOwner?.Memory ?? ReadOnlyMemory<byte>.Empty;

    internal WebElementObject(IWebElement model)
    {
        Model = model;
        ElementId = Guid.NewGuid();
    }

    public void SetOuterHtml(ReadOnlySpan<char> value)
    {
        _outerHtmlBytesOwner?.Dispose();
        _outerHtmlBytesOwner = ProcessContent(value);
    }

    public void SetInnerText(ReadOnlySpan<char> value)
    {
        _innerTextBytesOwner?.Dispose();
        _innerTextBytesOwner = ProcessContent(value);
    }

    private static IMemoryOwner<byte>? ProcessContent(ReadOnlySpan<char> content)
    {
        if (content.IsEmpty)
            return null;

        var memoryOwner = MemoryPool<byte>.Shared.Rent(Encoding.UTF8.GetByteCount(content));
        Encoding.UTF8.GetBytes(content, memoryOwner.Memory.Span);
        return memoryOwner;
    }

    public void Dispose()
    {
        _outerHtmlBytesOwner?.Dispose();
        _innerTextBytesOwner?.Dispose();
        GC.SuppressFinalize(this);
    }
}
