using System.Runtime.InteropServices;
using System.Text;

namespace ThorVGSharp;

/// <summary>
/// Helper class for string conversions between managed and unmanaged code.
/// </summary>
internal static class StringHelper
{
    private const int StackAllocThreshold = 256;

    /// <summary>
    /// Encodes a string to a null-terminated UTF-8 byte span on the stack (or heap for large strings)
    /// and invokes the callback with the resulting pointer.
    /// Zero-allocation for strings that fit within the stack threshold.
    /// </summary>
    public static unsafe void WithNativeString(string? text, delegate*<sbyte*, void> action)
    {
        if (string.IsNullOrEmpty(text))
        {
            action(null);
            return;
        }

        int maxByteCount = Encoding.UTF8.GetMaxByteCount(text.Length) + 1; // +1 for null terminator

        if (maxByteCount <= StackAllocThreshold)
        {
            byte* buffer = stackalloc byte[maxByteCount];
            int written = Encoding.UTF8.GetBytes(text, new Span<byte>(buffer, maxByteCount - 1));
            buffer[written] = 0; // null terminator
            action((sbyte*)buffer);
        }
        else
        {
            byte[] rented = new byte[maxByteCount];
            fixed (byte* buffer = rented)
            {
                int written = Encoding.UTF8.GetBytes(text, new Span<byte>(buffer, maxByteCount - 1));
                buffer[written] = 0; // null terminator
                action((sbyte*)buffer);
            }
        }
    }

    /// <summary>
    /// Encodes a string to null-terminated UTF-8 bytes, writing into the provided span.
    /// Returns the number of bytes written (including null terminator).
    /// The caller must ensure the span is large enough (use GetMaxByteCount + 1).
    /// </summary>
    public static int EncodeToUtf8(string text, Span<byte> destination)
    {
        int written = Encoding.UTF8.GetBytes(text.AsSpan(), destination);
        destination[written] = 0; // null terminator
        return written + 1;
    }

    /// <summary>
    /// Returns the maximum number of bytes needed to encode a string as null-terminated UTF-8.
    /// </summary>
    public static int GetMaxByteCount(string text)
    {
        return Encoding.UTF8.GetMaxByteCount(text.Length) + 1; // +1 for null terminator
    }

    /// <summary>
    /// Converts an sbyte* pointer to a managed string.
    /// </summary>
    public static unsafe string? FromNativeString(sbyte* ptr)
    {
        if (ptr == null)
            return null;

        return Marshal.PtrToStringUTF8((IntPtr)ptr);
    }
}
