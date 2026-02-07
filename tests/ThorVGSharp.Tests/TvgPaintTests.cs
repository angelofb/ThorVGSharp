// Tests adapted from external/thorvg/test/testPaint.cpp
namespace ThorVGSharp.Tests;

public class TvgPaintTests : IDisposable
{
    public TvgPaintTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void CustomTransformation()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        // Verify default transform (identity matrix)
        var m1 = shape.GetTransform();
        Assert.Equal(1.0f, m1.E11, 6);
        Assert.Equal(0.0f, m1.E12, 6);
        Assert.Equal(0.0f, m1.E13, 6);
        Assert.Equal(0.0f, m1.E21, 6);
        Assert.Equal(1.0f, m1.E22, 6);
        Assert.Equal(0.0f, m1.E23, 6);
        Assert.Equal(0.0f, m1.E31, 6);
        Assert.Equal(0.0f, m1.E32, 6);
        Assert.Equal(1.0f, m1.E33, 6);

        // Custom transform
        var m2 = new TvgMatrix(1.0f, 2.0f, 3.0f, 4.0f, 0.0f, -4.0f, -3.0f, -2.0f, -1.0f);
        shape.SetTransform(m2);

        var m3 = shape.GetTransform();
        Assert.Equal(m2.E11, m3.E11, 6);
        Assert.Equal(m2.E12, m3.E12, 6);
        Assert.Equal(m2.E13, m3.E13, 6);
        Assert.Equal(m2.E21, m3.E21, 6);
        Assert.Equal(m2.E22, m3.E22, 6);
        Assert.Equal(m2.E23, m3.E23, 6);
        Assert.Equal(m2.E31, m3.E31, 6);
        Assert.Equal(m2.E32, m3.E32, 6);
        Assert.Equal(m2.E33, m3.E33, 6);

        // It's not allowed if the custom transform is applied
        Assert.Throws<TvgInsufficientConditionException>(() => shape.Translate(155.0f, -155.0f));
        Assert.Throws<TvgInsufficientConditionException>(() => shape.Scale(4.7f));
        Assert.Throws<TvgInsufficientConditionException>(() => shape.Rotate(45.0f));

        // Verify transform is not modified
        var m4 = shape.GetTransform();
        Assert.Equal(m2.E11, m4.E11, 6);
        Assert.Equal(m2.E12, m4.E12, 6);
        Assert.Equal(m2.E13, m4.E13, 6);
        Assert.Equal(m2.E21, m4.E21, 6);
        Assert.Equal(m2.E22, m4.E22, 6);
        Assert.Equal(m2.E23, m4.E23, 6);
        Assert.Equal(m2.E31, m4.E31, 6);
        Assert.Equal(m2.E32, m4.E32, 6);
        Assert.Equal(m2.E33, m4.E33, 6);
    }

    [Fact]
    public void BasicTransformation()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.Translate(155.0f, -155.0f);
        shape.Rotate(45.0f);
        shape.Scale(4.7f);

        var m = shape.GetTransform();
        Assert.Equal(3.323402f, m.E11, 5);
        Assert.Equal(-3.323401f, m.E12, 5);
        Assert.Equal(155.0f, m.E13, 5);
        Assert.Equal(3.323401f, m.E21, 5);
        Assert.Equal(3.323402f, m.E22, 5);
        Assert.Equal(-155.0f, m.E23, 5);
        Assert.Equal(0.0f, m.E31, 5);
        Assert.Equal(0.0f, m.E32, 5);
        Assert.Equal(1.0f, m.E33, 5);
    }

    [Fact]
    public void Opacity()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        Assert.Equal(255, shape.GetOpacity());

        shape.SetOpacity(155);
        Assert.Equal(155, shape.GetOpacity());

        shape.SetOpacity(255); // -1 in C++ wraps to 255
        Assert.Equal(255, shape.GetOpacity());

        shape.SetOpacity(0);
        Assert.Equal(0, shape.GetOpacity());
    }

    [Fact]
    public void Visibility()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        Assert.True(shape.GetVisible());

        shape.SetVisible(false);
        Assert.False(shape.GetVisible());

        shape.SetVisible(false);
        Assert.False(shape.GetVisible());

        shape.SetVisible(true);
        Assert.True(shape.GetVisible());
    }

    [Fact]
    public void BoundingBox()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        var buffer = new uint[500 * 500];
        canvas.SetTarget(buffer, 500, 500, 500, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        canvas.Add(shape);

        // Negative - insufficient condition before shape is defined
        Assert.Throws<TvgInsufficientConditionException>(() => shape.GetBounds());

        // Case 1
        shape.AppendRect(0.0f, 10.0f, 20.0f, 100.0f, 50.0f, 50.0f);
        shape.Translate(100.0f, 111.0f);

        canvas.Update();

        // Positive
        var (x, y, w, h) = shape.GetBounds();
        Assert.Equal(100.0f, x);
        Assert.Equal(121.0f, y);
        Assert.Equal(20.0f, w);
        Assert.Equal(100.0f, h);

        var pts = shape.GetOrientedBounds();
        Assert.Equal(4, pts.Length);
        Assert.Equal(100.0f, pts[0].X);
        Assert.Equal(100.0f, pts[3].X);
        Assert.Equal(121.0f, pts[0].Y);
        Assert.Equal(121.0f, pts[1].Y);
        Assert.Equal(120.0f, pts[1].X);
        Assert.Equal(120.0f, pts[2].X);
        Assert.Equal(221.0f, pts[2].Y);
        Assert.Equal(221.0f, pts[3].Y);

        canvas.Sync();

        // Case 2
        shape.Reset();
        shape.MoveTo(0.0f, 10.0f);
        shape.LineTo(20.0f, 210.0f);
        var identity = new TvgMatrix(1.0f, 0.0f, 0.0f, 0.0f, 1.0f, 0.0f, 0.0f, 0.0f, 1.0f);
        shape.SetTransform(identity);

        canvas.Update();

        (x, y, w, h) = shape.GetBounds();
        Assert.Equal(0.0f, x);
        Assert.Equal(10.0f, y);
        Assert.Equal(20.0f, w);
        Assert.Equal(200.0f, h);

        pts = shape.GetOrientedBounds();
        Assert.Equal(0.0f, pts[0].X);
        Assert.Equal(0.0f, pts[3].X);
        Assert.Equal(10.0f, pts[0].Y);
        Assert.Equal(10.0f, pts[1].Y);
        Assert.Equal(20.0f, pts[1].X);
        Assert.Equal(20.0f, pts[2].X);
        Assert.Equal(210.0f, pts[2].Y);
        Assert.Equal(210.0f, pts[3].Y);

        canvas.Sync();

        // Case 3
        shape.Reset();
        shape.MoveTo(10, 10);
        shape.LineTo(190, 10);
        shape.SetStrokeWidth(12.0f);
        shape.SetStrokeColor(255, 0, 0, 255);

        canvas.Update();

        (x, y, w, h) = shape.GetBounds();
        Assert.Equal(4.0f, x);
        Assert.Equal(4.0f, y);
        Assert.Equal(12.0f, h);
        Assert.Equal(192.0f, w);

        pts = shape.GetOrientedBounds();
        Assert.Equal(4.0f, pts[0].X);
        Assert.Equal(4.0f, pts[3].X);
        Assert.Equal(4.0f, pts[0].Y);
        Assert.Equal(4.0f, pts[1].Y);
        Assert.Equal(196.0f, pts[1].X);
        Assert.Equal(196.0f, pts[2].X);
        Assert.Equal(16.0f, pts[2].Y);
        Assert.Equal(16.0f, pts[3].Y);
    }

    [Fact]
    public void Intersection()
    {
        using var canvas = TvgCanvasSoftware.Create();
        Assert.NotNull(canvas);

        var buffer = new uint[200 * 200];
        canvas.SetTarget(buffer, 200, 200, 200, TvgColorSpace.Argb8888);

        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        shape.AppendRect(50, 50, 100, 100);
        shape.SetFillColor(255, 0, 0, 255);

        canvas.Add(shape);
        canvas.Draw();

        // Case1. Fully contained
        Assert.True(shape.Intersects(0, 0, 200, 200));

        // Case2. Partially overlapping
        Assert.True(shape.Intersects(25, 25, 50, 50));
        Assert.True(shape.Intersects(125, 125, 50, 50));

        // Case3. Edge-touching
        Assert.True(shape.Intersects(49, 49, 2, 2));
        Assert.True(shape.Intersects(149, 149, 2, 2));

        // Case4. Fully separated
        Assert.False(shape.Intersects(0, 0, 25, 25));
        Assert.False(shape.Intersects(175, 175, 25, 25));
    }

    [Fact]
    public void Duplication()
    {
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        // Setup paint properties
        shape.SetOpacity(0);
        shape.Translate(200.0f, 100.0f);
        shape.Scale(2.2f);
        shape.Rotate(90.0f);

        using var comp = TvgShape.Create();
        Assert.NotNull(comp);
        shape.SetClip(comp);

        // Duplication
        using var dup = shape.Duplicate();
        Assert.NotNull(dup);

        // Compare properties
        Assert.Equal(0, dup.GetOpacity());

        var m = dup.GetTransform();
        Assert.Equal(0.0f, m.E11, 5);
        Assert.Equal(-2.2f, m.E12, 5);
        Assert.Equal(200.0f, m.E13, 5);
        Assert.Equal(2.2f, m.E21, 5);
        Assert.Equal(0.0f, m.E22, 5);
        Assert.Equal(100.0f, m.E23, 5);
        Assert.Equal(0.0f, m.E31, 5);
        Assert.Equal(0.0f, m.E32, 5);
        Assert.Equal(1.0f, m.E33, 5);
    }
}
