using System.Numerics;

namespace ThorVGSharp.Sample.Janitor;

internal static class MathHelper
{
    public static float Length2(Vector2 vec)
    {
        return vec.X * vec.X + vec.Y * vec.Y;
    }

    public static bool Intersect(Vector2 a, Vector2 b, float distSquared)
    {
        return Length2(a - b) < distSquared;
    }

    public static Vector2 Normalize(Vector2 vec)
    {
        var length = MathF.Sqrt(Length2(vec));
        return length > 0 ? vec / length : vec;
    }

    public static Vector2 Extend(Vector2 vec, float targetLength)
    {
        var mag = MathF.Sqrt(Length2(vec));
        return mag > 0 ? vec * (targetLength / mag) : vec;
    }

    public static T Lerp<T>(T start, T end, float t) where T : struct
    {
        return start switch
        {
            float f => (T)(object)(f + (((float)(object)end!) - f) * t),
            Vector2 v => (T)(object)(v + (((Vector2)(object)end!) - v) * t),
            _ => throw new NotSupportedException($"Type {typeof(T)} not supported for lerp")
        };
    }

    public static float S(float value, float scale) => value * scale;
}
