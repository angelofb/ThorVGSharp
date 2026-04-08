using System.Runtime.InteropServices;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgTextMetrics
{
    public readonly float Ascent;
    public readonly float Descent;
    public readonly float LineGap;
    public readonly float Advance;

    public TvgTextMetrics(float ascent, float descent, float lineGap, float advance)
    {
        Ascent = ascent;
        Descent = descent;
        LineGap = lineGap;
        Advance = advance;
    }
}
