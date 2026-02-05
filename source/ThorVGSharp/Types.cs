namespace ThorVGSharp;

public enum TvgType
{
    Undef = 0,
    Shape,
    Scene,
    Picture,
    Text,
    LinearGrad = 10,
    RadialGrad,
}

public enum TvgTextWrap
{
    None = 0,
    Character,
    Word,
    Smart,
    Ellipsis,
    Hyphenation,
}

public enum TvgStrokeJoin
{
    Miter = 0,
    Round,
    Bevel,
}

public enum TvgStrokeFill
{
    Pad = 0,
    Reflect,
    Repeat,
}

public enum TvgStrokeCap
{
    Butt = 0,
    Round,
    Square,
}

public enum TvgMaskMethod
{
    None = 0,
    Alpha,
    InverseAlpha,
    Luma,
    InverseLuma,
    Add,
    Subtract,
    Intersect,
    Difference,
    Lighten,
    Darken,
}

public enum TvgFillRule
{
    NonZero = 0,
    EvenOdd,
}

public enum TvgEngineOption
{
    None = 0,
    Default = 1 << 0,
    SmartRender = 1 << 1,
}

public enum TvgColorSpace
{
    Abgr8888 = 0,
    Argb8888,
    Abgr8888s,
    Argb8888s,
    Unknown = 255,
}

public enum TvgBlendMethod
{
    Normal = 0,
    Multiply,
    Screen,
    Overlay,
    Darken,
    Lighten,
    Colordodge,
    Colorburn,
    Hardlight,
    Softlight,
    Difference,
    Exclusion,
    Hue,
    Saturation,
    Color,
    Luminosity,
    Add,
    Composition = 255,
}

public enum TvgPathCommand : byte
{
    Close = 0,
    MoveTo,
    LineTo,
    CubicTo,
}
