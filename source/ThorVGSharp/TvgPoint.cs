using System.Runtime.InteropServices;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgPoint
{
    public readonly float X;
    public readonly float Y;

    public TvgPoint(float x, float y)
    {
        X = x;
        Y = y;
    }
}
