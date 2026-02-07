using System.Runtime.InteropServices;

namespace ThorVGSharp;

[StructLayout(LayoutKind.Sequential)]
public readonly partial struct TvgMatrix
{
    public readonly float E11;
    public readonly float E12;
    public readonly float E13;
    public readonly float E21;
    public readonly float E22;
    public readonly float E23;
    public readonly float E31;
    public readonly float E32;
    public readonly float E33;

    public TvgMatrix(float e11, float e12, float e13, float e21, float e22, float e23, float e31, float e32, float e33)
    {
        E11 = e11;
        E12 = e12;
        E13 = e13;
        E21 = e21;
        E22 = e22;
        E23 = e23;
        E31 = e31;
        E32 = e32;
        E33 = e33;
    }
}
