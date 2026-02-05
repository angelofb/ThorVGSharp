namespace ThorVGSharp.Interop;

internal partial struct Tvg_Color_Stop
{
    public float offset;

    [NativeTypeName("uint8_t")]
    public byte r;

    [NativeTypeName("uint8_t")]
    public byte g;

    [NativeTypeName("uint8_t")]
    public byte b;

    [NativeTypeName("uint8_t")]
    public byte a;
}
