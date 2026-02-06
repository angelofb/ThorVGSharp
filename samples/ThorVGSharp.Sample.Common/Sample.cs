namespace ThorVGSharp.Sample.Common;

public static class Samples
{
    const int N_INTERACTIVE = 5_000; // For interactive UI (much faster)

    /// <summary>
    /// Optimized version that draws to a reusable scene (for interactive UI)
    /// </summary>
    public static void DrawThorToScene(TvgScene scene, int width, int height, int count = N_INTERACTIVE)
    {
        ArgumentNullException.ThrowIfNull(scene);

        // Use reduced count for better interactive performance
        for (int i = 0; i < count; i++)
        {
            using var item = TvgShape.Create();
            if (item == null)
                continue;

            var x = Random.Shared.Next(0, width);
            var y = Random.Shared.Next(0, height);
            var r = Random.Shared.Next(10, 50);

            item.AppendCircle(x, y, r, r);

            var r1 = (byte)Random.Shared.Next(0, 255);
            var g1 = (byte)Random.Shared.Next(0, 255);
            var b1 = (byte)Random.Shared.Next(0, 255);
            item.SetFillColor(r1, g1, b1);

            var r2 = (byte)Random.Shared.Next(0, 255);
            var g2 = (byte)Random.Shared.Next(0, 255);
            var b2 = (byte)Random.Shared.Next(0, 255);
            item.SetStrokeColor(r2, g2, b2);

            var s = Random.Shared.Next(1, 5);
            item.SetStrokeWidth(s);

            scene.Add(item);
        }
    }
}
