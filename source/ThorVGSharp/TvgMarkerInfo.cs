namespace ThorVGSharp;

/// <summary>
/// Marker information for a Lottie animation.
/// </summary>
public readonly partial struct TvgMarkerInfo
{
    public readonly string Name;
    public readonly float Begin;
    public readonly float End;

    public TvgMarkerInfo(string name, float begin, float end)
    {
        ArgumentNullException.ThrowIfNull(name);
        Name = name;
        Begin = begin;
        End = end;
    }
}
