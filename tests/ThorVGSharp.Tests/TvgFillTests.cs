// Tests adapted from external/thorvg/test/testFill.cpp
namespace ThorVGSharp.Tests;

public class TvgFillTests : IDisposable
{
    public TvgFillTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void FillingCreation()
    {
        using var linear = TvgLinearGradient.Create();
        Assert.NotNull(linear);
        Assert.Equal(TvgType.LinearGrad, linear.GetGradientType());

        using var radial = TvgRadialGradient.Create();
        Assert.NotNull(radial);
        Assert.Equal(TvgType.RadialGrad, radial.GetGradientType());
    }

    [Fact]
    public void CommonFilling()
    {
        using var fill = TvgLinearGradient.Create();
        Assert.NotNull(fill);

        // Options
        Assert.Equal(TvgStrokeFill.Pad, fill.GetSpread());
        fill.SetSpread(TvgStrokeFill.Pad);
        fill.SetSpread(TvgStrokeFill.Reflect);
        fill.SetSpread(TvgStrokeFill.Repeat);
        Assert.Equal(TvgStrokeFill.Repeat, fill.GetSpread());

        // ColorStops
        var cs = fill.GetColorStops();
        Assert.Empty(cs);

        var cs2 = new TvgColorStop[]
        {
            new(0.0f, 0, 0, 0, 0),
            new(0.2f, 50, 25, 50, 25),
            new(0.5f, 100, 100, 100, 125),
            new(1.0f, 255, 255, 255, 255)
        };

        Assert.Throws<ArgumentException>(() => fill.SetColorStops(Array.Empty<TvgColorStop>()));
        fill.SetColorStops(cs2);
        cs = fill.GetColorStops();
        Assert.Equal(4, cs.Length);

        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(cs2[i].Offset, cs[i].Offset);
            Assert.Equal(cs2[i].R, cs[i].R);
            Assert.Equal(cs2[i].G, cs[i].G);
            Assert.Equal(cs2[i].B, cs[i].B);
        }

        // Reset ColorStop - in C++ this is done by passing nullptr, 0
        // In C# we can't clear, but we can set new ones
        var emptyCs = new TvgColorStop[]
        {
            new(0.0f, 0, 0, 0, 0)
        };
        fill.SetColorStops(emptyCs);
        cs = fill.GetColorStops();
        Assert.Single(cs);

        // Set to Shape
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);

        shape.SetFillGradient(fill);
        var retrievedFill = shape.GetFillGradient();
        Assert.NotNull(retrievedFill);
    }

    [Fact]
    public void FillTransformation()
    {
        using var fill = TvgLinearGradient.Create();
        Assert.NotNull(fill);

        // no transformation
        var mGet = fill.GetTransform();
        Assert.Equal(1.0f, mGet.E11, 6);
        Assert.Equal(0.0f, mGet.E12, 6);
        Assert.Equal(0.0f, mGet.E13, 6);
        Assert.Equal(0.0f, mGet.E21, 6);
        Assert.Equal(1.0f, mGet.E22, 6);
        Assert.Equal(0.0f, mGet.E23, 6);
        Assert.Equal(0.0f, mGet.E31, 6);
        Assert.Equal(0.0f, mGet.E32, 6);
        Assert.Equal(1.0f, mGet.E33, 6);

        var mSet = new TvgMatrix(1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, -7.7f, -8.8f, -9.9f);
        fill.SetTransform(mSet);

        // transformation was set
        mGet = fill.GetTransform();
        Assert.Equal(mSet.E11, mGet.E11, 6);
        Assert.Equal(mSet.E12, mGet.E12, 6);
        Assert.Equal(mSet.E13, mGet.E13, 6);
        Assert.Equal(mSet.E21, mGet.E21, 6);
        Assert.Equal(mSet.E22, mGet.E22, 6);
        Assert.Equal(mSet.E23, mGet.E23, 6);
        Assert.Equal(mSet.E31, mGet.E31, 6);
        Assert.Equal(mSet.E32, mGet.E32, 6);
        Assert.Equal(mSet.E33, mGet.E33, 6);
    }

    [Fact]
    public void LinearFilling()
    {
        using var fill = TvgLinearGradient.Create();
        Assert.NotNull(fill);

        fill.SetLinear(0, 0, 0, 0);

        var (x1, y1, x2, y2) = fill.GetLinear();
        Assert.Equal(0.0f, x1);
        Assert.Equal(0.0f, y1);
        Assert.Equal(0.0f, x2);
        Assert.Equal(0.0f, y2);

        fill.SetLinear(-1.0f, -1.0f, 100.0f, 100.0f);
        (x1, y1, x2, y2) = fill.GetLinear();
        Assert.Equal(-1.0f, x1);
        Assert.Equal(-1.0f, y1);
        Assert.Equal(100.0f, x2);
        Assert.Equal(100.0f, y2);
    }

    [Fact]
    public void RadialFilling()
    {
        using var fill = TvgRadialGradient.Create();
        Assert.NotNull(fill);

        Assert.Throws<TvgInvalidArgumentException>(() => fill.SetRadial(0, 0, -1, 0, 0, 0));
        Assert.Throws<TvgInvalidArgumentException>(() => fill.SetRadial(0, 0, 0, 0, 0, -1));

        fill.SetRadial(100, 120, 50, 10, 20, 5);

        var (cx, cy, r, fx, fy, fr) = fill.GetRadial();
        Assert.Equal(100.0f, cx);
        Assert.Equal(120.0f, cy);
        Assert.Equal(50.0f, r);
        Assert.Equal(10.0f, fx);
        Assert.Equal(20.0f, fy);
        Assert.Equal(5.0f, fr);

        fill.SetRadial(0, 0, 0, 0, 0, 0);
        (cx, cy, r, fx, fy, fr) = fill.GetRadial();
        Assert.Equal(0.0f, cx);
        Assert.Equal(0.0f, cy);
        Assert.Equal(0.0f, r);
        Assert.Equal(0.0f, fx);
        Assert.Equal(0.0f, fy);
        Assert.Equal(0.0f, fr);
    }

    [Fact]
    public void LinearFillingDuplication()
    {
        using var fill = TvgLinearGradient.Create();
        Assert.NotNull(fill);

        // Setup
        var cs = new TvgColorStop[]
        {
            new(0.0f, 0, 0, 0, 0),
            new(0.2f, 50, 25, 50, 25),
            new(0.5f, 100, 100, 100, 125),
            new(1.0f, 255, 255, 255, 255)
        };

        fill.SetColorStops(cs);
        fill.SetSpread(TvgStrokeFill.Reflect);
        fill.SetLinear(-10.0f, 10.0f, 100.0f, 120.0f);

        var m = new TvgMatrix(1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, -7.7f, -8.8f, -9.9f);
        fill.SetTransform(m);

        // Duplication
        using var dup = (TvgLinearGradient?)fill.Duplicate();
        Assert.NotNull(dup);

        Assert.Equal(TvgStrokeFill.Reflect, dup.GetSpread());

        var (x1, y1, x2, y2) = dup.GetLinear();
        Assert.Equal(-10.0f, x1);
        Assert.Equal(10.0f, y1);
        Assert.Equal(100.0f, x2);
        Assert.Equal(120.0f, y2);

        var cs2 = dup.GetColorStops();
        Assert.Equal(4, cs2.Length);

        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(cs[i].Offset, cs2[i].Offset);
            Assert.Equal(cs[i].R, cs2[i].R);
            Assert.Equal(cs[i].G, cs2[i].G);
            Assert.Equal(cs[i].B, cs2[i].B);
        }

        var mDup = dup.GetTransform();
        Assert.Equal(m.E11, mDup.E11, 6);
        Assert.Equal(m.E12, mDup.E12, 6);
        Assert.Equal(m.E13, mDup.E13, 6);
        Assert.Equal(m.E21, mDup.E21, 6);
        Assert.Equal(m.E22, mDup.E22, 6);
        Assert.Equal(m.E23, mDup.E23, 6);
        Assert.Equal(m.E31, mDup.E31, 6);
        Assert.Equal(m.E32, mDup.E32, 6);
        Assert.Equal(m.E33, mDup.E33, 6);
    }

    [Fact]
    public void RadialFillingDuplication()
    {
        using var fill = TvgRadialGradient.Create();
        Assert.NotNull(fill);

        // Setup
        var cs = new TvgColorStop[]
        {
            new(0.0f, 0, 0, 0, 0),
            new(0.2f, 50, 25, 50, 25),
            new(0.5f, 100, 100, 100, 125),
            new(1.0f, 255, 255, 255, 255)
        };

        fill.SetColorStops(cs);
        fill.SetSpread(TvgStrokeFill.Reflect);
        fill.SetRadial(100.0f, 120.0f, 50.0f, 10.0f, 20.0f, 5.0f);

        var m = new TvgMatrix(1.1f, 2.2f, 3.3f, 4.4f, 5.5f, 6.6f, -7.7f, -8.8f, -9.9f);
        fill.SetTransform(m);

        // Duplication
        using var dup = (TvgRadialGradient?)fill.Duplicate();
        Assert.NotNull(dup);

        Assert.Equal(TvgStrokeFill.Reflect, dup.GetSpread());

        var (cx, cy, r, fx, fy, fr) = dup.GetRadial();
        Assert.Equal(100.0f, cx);
        Assert.Equal(120.0f, cy);
        Assert.Equal(50.0f, r);
        Assert.Equal(10.0f, fx);
        Assert.Equal(20.0f, fy);
        Assert.Equal(5.0f, fr);

        var cs2 = dup.GetColorStops();
        Assert.Equal(4, cs2.Length);

        for (int i = 0; i < 4; i++)
        {
            Assert.Equal(cs[i].Offset, cs2[i].Offset);
            Assert.Equal(cs[i].R, cs2[i].R);
            Assert.Equal(cs[i].G, cs2[i].G);
            Assert.Equal(cs[i].B, cs2[i].B);
        }

        var mDup = dup.GetTransform();
        Assert.Equal(m.E11, mDup.E11, 6);
        Assert.Equal(m.E12, mDup.E12, 6);
        Assert.Equal(m.E13, mDup.E13, 6);
        Assert.Equal(m.E21, mDup.E21, 6);
        Assert.Equal(m.E22, mDup.E22, 6);
        Assert.Equal(m.E23, mDup.E23, 6);
        Assert.Equal(m.E31, mDup.E31, 6);
        Assert.Equal(m.E32, mDup.E32, 6);
        Assert.Equal(m.E33, mDup.E33, 6);
    }
}
