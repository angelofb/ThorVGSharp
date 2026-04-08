namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgAccessorTests : IDisposable
{
    public TvgAccessorTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void GenerateId_SameInput_ReturnsSameValue()
    {
        uint id1 = TvgAccessor.GenerateId("layer.sample");
        uint id2 = TvgAccessor.GenerateId("layer.sample");

        Assert.Equal(id1, id2);
    }

    [Fact]
    public void Set_ThrowsOnNullArguments()
    {
        using var accessor = TvgAccessor.Create();
        using var shape = TvgShape.Create();

        Assert.Throws<ArgumentNullException>(() => accessor.Set(null!, (_, _) => true));
        Assert.Throws<ArgumentNullException>(() => accessor.Set(shape, null!));
    }

    [Fact]
    public void Set_VisitsSceneHierarchy()
    {
        using var root = TvgScene.Create();
        using var child = TvgScene.Create();
        using var shape = TvgShape.Create();
        using var accessor = TvgAccessor.Create();

        shape.AppendRect(0, 0, 50, 50);
        child.Add(shape);
        root.Add(child);

        var visited = new List<TvgType>();
        accessor.Set(root, (paint, _) =>
        {
            visited.Add(paint.GetPaintType());
            paint.Dispose();
            return true;
        });

        Assert.NotEmpty(visited);
        Assert.Contains(TvgType.Shape, visited);
    }
}
