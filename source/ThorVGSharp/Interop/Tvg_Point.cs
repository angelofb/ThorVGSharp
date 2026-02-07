using System.Runtime.InteropServices;

namespace ThorVGSharp.Interop;

[StructLayout(LayoutKind.Sequential)]
internal partial struct Tvg_Point
{
    public float x;

    public float y;
}
