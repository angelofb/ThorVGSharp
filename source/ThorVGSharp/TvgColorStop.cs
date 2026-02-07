using System.Runtime.InteropServices;

using ThorVGSharp.Internal.Attributes;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgColorStop
{
    public readonly float Offset;

    [NativeTypeName("uint8_t")]
    public readonly byte R;

    [NativeTypeName("uint8_t")]
    public readonly byte G;

    [NativeTypeName("uint8_t")]
    public readonly byte B;

    [NativeTypeName("uint8_t")]
    public readonly byte A;

    public TvgColorStop(float offset, byte r, byte g, byte b, byte a)
    {
        Offset = offset;
        R = r;
        G = g;
        B = b;
        A = a;
    }
}
