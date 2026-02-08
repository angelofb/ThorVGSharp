using System.Reflection;

namespace ThorVGSharp.Sample.Janitor;

internal static class Assets
{
    private static readonly Assembly Assembly = typeof(Assets).Assembly;

    public static byte[] LoadFont()
    {
        return LoadResource("ThorVGSharp.Sample.Janitor.Assets.font.ttf");
    }

    public static byte[] LoadHaloImage()
    {
        return LoadResource("ThorVGSharp.Sample.Janitor.Assets.halo.jpg");
    }

    public static string LoadLifeIconSvg()
    {
        using var stream = Assembly.GetManifestResourceStream("ThorVGSharp.Sample.Janitor.Assets.life_icon.svg");
        if (stream == null) throw new FileNotFoundException("Life icon SVG not found");

        using var reader = new StreamReader(stream);
        return reader.ReadToEnd();
    }

    private static byte[] LoadResource(string resourceName)
    {
        using var stream = Assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
        {
            // List available resources for debugging
            var resources = Assembly.GetManifestResourceNames();
            throw new FileNotFoundException(
                $"Resource '{resourceName}' not found. Available: {string.Join(", ", resources)}");
        }

        using var ms = new MemoryStream();
        stream.CopyTo(ms);
        return ms.ToArray();
    }

    public const string FontName = "04B_30__";
}
