using System.Runtime.InteropServices;

namespace ThorVGSharp.Interop;

[StructLayout(LayoutKind.Sequential)]
internal partial struct Tvg_Matrix
{
    public float e11;

    public float e12;

    public float e13;

    public float e21;

    public float e22;

    public float e23;

    public float e31;

    public float e32;

    public float e33;
}
