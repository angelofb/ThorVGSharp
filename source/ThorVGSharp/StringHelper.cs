using System.Runtime.InteropServices;
using System.Text;

namespace ThorVGSharp;

/// <summary>
/// Helper class for string conversions between managed and unmanaged code.
/// </summary>
internal static class StringHelper
{
    /// <summary>
    /// Converts a managed string to a null-terminated UTF-8 sbyte* pointer.
    /// </summary>
    public static unsafe sbyte* ToNativeString(string? text, byte[] buffer)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        var bytes = Encoding.UTF8.GetBytes(text + '\0');
        if (bytes.Length > buffer.Length)
            throw new ArgumentException("Buffer too small for string conversion");

        fixed (byte* src = bytes)
        fixed (byte* dest = buffer)
        {
            Buffer.MemoryCopy(src, dest, buffer.Length, bytes.Length);
            return (sbyte*)dest;
        }
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
