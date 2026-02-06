// Tests adapted from external/thorvg/test/testShape.cpp
namespace ThorVGSharp.Tests;

public class TvgShapeTests : IDisposable
{
    public TvgShapeTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void ShapeCreation()
    {
        var shape = TvgShape.Create();
        Assert.NotNull(shape);

        Assert.Equal(TvgType.Shape, shape.GetPaintType());

        shape.Dispose();
    }

    [Fact]
    public void AppendingCommands()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.Close();

        shape.MoveTo(100, 100);
        shape.MoveTo(99999999.0f, -99999999.0f);
        shape.MoveTo(0, 0);

        shape.LineTo(120, 140);
        shape.LineTo(99999999.0f, -99999999.0f);
        shape.LineTo(0, 0);

        shape.CubicTo(0, 0, 0, 0, 0, 0);
        shape.CubicTo(0, 0, 99999999.0f, -99999999.0f, 0, 0);
        shape.CubicTo(0, 0, 99999999.0f, -99999999.0f, 99999999.0f, -99999999.0f);
        shape.CubicTo(99999999.0f, -99999999.0f, 99999999.0f, -99999999.0f, 99999999.0f, -99999999.0f);

        shape.Close();

        shape.Reset();
        shape.Reset(); // Should not throw
    }

    [Fact]
    public void AppendingShapes()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.MoveTo(100, 100);
        shape.LineTo(120, 140);

        shape.AppendRect(0, 0, 0, 0, 0, 0);
        shape.AppendRect(0, 0, 99999999.0f, -99999999.0f, 0, 0);
        shape.AppendRect(0, 0, 0, 0, -99999999.0f, 99999999.0f);
        shape.AppendRect(99999999.0f, -99999999.0f, 99999999.0f, -99999999.0f, 99999999.0f, -99999999.0f);

        shape.AppendCircle(0, 0, 0, 0);
        shape.AppendCircle(-99999999.0f, 99999999.0f, 0, 0);
        shape.AppendCircle(-99999999.0f, 99999999.0f, -99999999.0f, 99999999.0f);
    }

    [Fact]
    public void AppendingPaths()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        // Negative cases
        Assert.Throws<ArgumentNullException>(() =>
            shape.AppendPath(null!, Array.Empty<(float, float)>()));
        Assert.Throws<ArgumentNullException>(() =>
            shape.AppendPath(Array.Empty<TvgPathCommand>(), null!));
        Assert.Throws<ArgumentException>(() =>
            shape.AppendPath(Array.Empty<TvgPathCommand>(), Array.Empty<(float, float)>()));

        var cmds = new[]
        {
            TvgPathCommand.Close,
            TvgPathCommand.MoveTo,
            TvgPathCommand.LineTo,
            TvgPathCommand.CubicTo,
            TvgPathCommand.Close
        };

        var pts = new[]
        {
            (100f, 100f),
            (200f, 200f),
            (10f, 10f),
            (20f, 20f),
            (30f, 30f)
        };

        shape.AppendPath(cmds, pts);

        var (commands, points) = shape.GetPath();
        Assert.Equal(5, commands.Length);
        Assert.Equal(5, points.Length);

        for (int i = 0; i < 5; i++)
        {
            Assert.Equal(cmds[i], commands[i]);
            Assert.Equal(pts[i].Item1, points[i].x);
            Assert.Equal(pts[i].Item2, points[i].y);
        }

        shape.Reset();
        var (emptyCommands, emptyPoints) = shape.GetPath();
        Assert.Empty(emptyCommands);
        Assert.Empty(emptyPoints);
    }

    [Fact]
    public void FillColor()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.SetFillColor(255, 0, 0, 255);

        var (r, g, b, a) = shape.GetFillColor();
        Assert.Equal(255, r);
        Assert.Equal(0, g);
        Assert.Equal(0, b);
        Assert.Equal(255, a);
    }

    [Fact]
    public void StrokeProperties()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        // Stroke width
        shape.SetStrokeWidth(5.0f);
        Assert.Equal(5.0f, shape.GetStrokeWidth());

        shape.SetStrokeWidth(0);
        Assert.Equal(0f, shape.GetStrokeWidth());

        shape.SetStrokeWidth(999.99f);
        Assert.Equal(999.99f, shape.GetStrokeWidth(), 2);

        // Stroke color
        shape.SetStrokeColor(0, 255, 0, 255);
        var (r, g, b, a) = shape.GetStrokeColor();
        Assert.Equal(0, r);
        Assert.Equal(255, g);
        Assert.Equal(0, b);
        Assert.Equal(255, a);
    }

    [Fact]
    public void StrokeCapJoin()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        // Stroke cap
        shape.SetStrokeCap(TvgStrokeCap.Round);
        Assert.Equal(TvgStrokeCap.Round, shape.GetStrokeCap());

        shape.SetStrokeCap(TvgStrokeCap.Square);
        Assert.Equal(TvgStrokeCap.Square, shape.GetStrokeCap());

        shape.SetStrokeCap(TvgStrokeCap.Butt);
        Assert.Equal(TvgStrokeCap.Butt, shape.GetStrokeCap());

        // Stroke join
        shape.SetStrokeJoin(TvgStrokeJoin.Bevel);
        Assert.Equal(TvgStrokeJoin.Bevel, shape.GetStrokeJoin());

        shape.SetStrokeJoin(TvgStrokeJoin.Round);
        Assert.Equal(TvgStrokeJoin.Round, shape.GetStrokeJoin());

        shape.SetStrokeJoin(TvgStrokeJoin.Miter);
        Assert.Equal(TvgStrokeJoin.Miter, shape.GetStrokeJoin());
    }

    [Fact]
    public void FillRule()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.SetFillRule(TvgFillRule.EvenOdd);
        Assert.Equal(TvgFillRule.EvenOdd, shape.GetFillRule());

        shape.SetFillRule(TvgFillRule.NonZero);
        Assert.Equal(TvgFillRule.NonZero, shape.GetFillRule());
    }
}
