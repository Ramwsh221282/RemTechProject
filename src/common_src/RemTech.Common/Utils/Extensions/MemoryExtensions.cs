using System.Buffers;
using System.Text;

namespace RemTechCommon.Utils.Extensions;

public static class MemoryExtensions
{
    public static IMemoryOwner<byte>? ToMemoryFromMemory(this Memory<byte> memory)
    {
        if (memory.IsEmpty)
            return null;

        IMemoryOwner<byte> owner = MemoryPool<byte>.Shared.Rent(memory.Length);
        memory.CopyTo(owner.Memory);
        return owner;
    }

    public static string ToStringFromMemory(this IMemoryOwner<byte>? memory)
    {
        if (memory == null)
            return string.Empty;
        return memory.Memory.IsEmpty ? string.Empty : Encoding.UTF8.GetString(memory.Memory.Span);
    }

    public static IMemoryOwner<byte>? ToMemoryFromString(this string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(value.Length);
        return MemoryPool<byte>.Shared.Rent(maxByteCount);
    }
}
