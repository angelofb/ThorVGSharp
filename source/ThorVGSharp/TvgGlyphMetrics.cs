using System.Runtime.InteropServices;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgGlyphMetrics
{
    public readonly float Advance;
    public readonly float Bearing;
    public readonly TvgPoint Min;
    public readonly TvgPoint Max;

    public TvgGlyphMetrics(float advance, float bearing, TvgPoint min, TvgPoint max)
    {
        Advance = advance;
        Bearing = bearing;
        Min = min;
        Max = max;
    }
}
