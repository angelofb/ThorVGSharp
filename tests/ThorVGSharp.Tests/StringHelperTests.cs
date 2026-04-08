using System.Runtime.InteropServices;

namespace ThorVGSharp.Tests;

public class StringHelperTests
{
    private static string? _capturedText;
    private static bool _capturedNull;

    [Fact]
    public void EncodeToUtf8_WritesNullTerminatedBytes()
    {
        Span<byte> buffer = stackalloc byte[StringHelper.GetMaxByteCount("ThorVG")];
        int written = StringHelper.EncodeToUtf8("ThorVG", buffer);

        Assert.True(written > 0);
        Assert.Equal(0, buffer[written - 1]);
    }

    [Fact]
    public void FromNativeString_NullPointer_ReturnsNull()
    {
        unsafe
        {
            Assert.Null(StringHelper.FromNativeString((sbyte*)0));
        }
    }

    [Fact]
    public void FromNativeString_ValidPointer_RoundTrips()
    {
        nint ptr = Marshal.StringToCoTaskMemUTF8("hello");
        try
        {
            unsafe
            {
                string? value = StringHelper.FromNativeString((sbyte*)ptr);
                Assert.Equal("hello", value);
            }
        }
        finally
        {
            Marshal.FreeCoTaskMem(ptr);
        }
    }

    [Fact]
    public unsafe void WithNativeString_HandlesNullAndEmpty()
    {
        _capturedText = null;
        _capturedNull = false;
        StringHelper.WithNativeString(null, &CapturePointer);
        Assert.True(_capturedNull);
        Assert.Null(_capturedText);

        _capturedText = null;
        _capturedNull = false;
        StringHelper.WithNativeString(string.Empty, &CapturePointer);
        Assert.True(_capturedNull);
        Assert.Null(_capturedText);
    }

    [Fact]
    public unsafe void WithNativeString_EncodesSmallAndLargeStrings()
    {
        _capturedText = null;
        _capturedNull = false;
        StringHelper.WithNativeString("small", &CapturePointer);
        Assert.False(_capturedNull);
        Assert.Equal("small", _capturedText);

        string large = new('x', 600);
        _capturedText = null;
        _capturedNull = false;
        StringHelper.WithNativeString(large, &CapturePointer);
        Assert.False(_capturedNull);
        Assert.Equal(large, _capturedText);
    }

    private static unsafe void CapturePointer(sbyte* ptr)
    {
        if (ptr == null)
        {
            _capturedNull = true;
            _capturedText = null;
            return;
        }

        _capturedNull = false;
        _capturedText = Marshal.PtrToStringUTF8((nint)ptr);
    }
}
