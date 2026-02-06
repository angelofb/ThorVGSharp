// Tests adapted from external/thorvg/test/testScene.cpp
namespace ThorVGSharp.Tests;

public class TvgSceneTests : IDisposable
{
    public TvgSceneTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void SceneCreation()
    {
        using var scene = TvgScene.Create();
        Assert.NotNull(scene);
        Assert.Equal(TvgType.Scene, scene.GetPaintType());
    }

    [Fact]
    public void PushingPaintsIntoScene()
    {
        using var scene = TvgScene.Create();
        Assert.NotNull(scene);

        // Create and add paints
        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        scene.Add(shape);

        using var picture = TvgPicture.Create();
        Assert.NotNull(picture);
        scene.Add(picture);

        using var picture2 = TvgPicture.Create();
        Assert.NotNull(picture2);
        scene.Add(picture2);

        // Null argument should throw
        Assert.Throws<ArgumentNullException>(() => scene.Add(null!));

        // Remove paints
        scene.Remove(shape);
        Assert.Throws<TvgInsufficientConditionException>(() => scene.Remove(shape)); // Already removed

        scene.Insert(shape, picture);
        scene.Remove(picture);
        Assert.Throws<TvgInsufficientConditionException>(() => scene.Remove(picture)); // Already removed

        scene.Remove(picture2);
        scene.Remove(shape);
    }

    [Fact]
    public void NestedScenes()
    {
        using var parentScene = TvgScene.Create();
        Assert.NotNull(parentScene);

        using var childScene = TvgScene.Create();
        Assert.NotNull(childScene);

        using var shape = TvgShape.Create();
        Assert.NotNull(shape);
        shape.AppendCircle(50, 50, 40, 40);
        shape.SetFillColor(255, 0, 0, 255);

        childScene.Add(shape);
        parentScene.Add(childScene);

        Assert.Equal(TvgType.Scene, parentScene.GetPaintType());
        Assert.Equal(TvgType.Scene, childScene.GetPaintType());
    }

    [Fact]
    public void SceneWithMultiplePaintTypes()
    {
        using var scene = TvgScene.Create();
        Assert.NotNull(scene);

        using var shape1 = TvgShape.Create();
        Assert.NotNull(shape1);
        shape1.AppendRect(0, 0, 50, 50, 0, 0);
        shape1.SetFillColor(255, 0, 0, 255);

        using var shape2 = TvgShape.Create();
        Assert.NotNull(shape2);
        shape2.AppendCircle(75, 75, 25, 25);
        shape2.SetFillColor(0, 255, 0, 255);

        using var picture = TvgPicture.Create();
        Assert.NotNull(picture);

        scene.Add(shape1);
        scene.Add(shape2);
        scene.Add(picture);
    }
}
