using ThorVGSharp.Interop;

namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgSaverTests : IDisposable
{
    public TvgSaverTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void Create_ReturnsSaver()
    {
        using var saver = TvgSaver.Create();
        Assert.NotNull(saver);
    }

    [Fact]
    public void Save_ThrowsOnNullArguments()
    {
        using var saver = TvgSaver.Create();

        Assert.Throws<ArgumentNullException>(() => saver.Save((TvgPaint)null!, "test.tvg"));
        Assert.Throws<ArgumentNullException>(() => saver.Save((TvgAnimation)null!, "test.gif"));
    }

    [Fact]
    public unsafe void SaveAndSync_WithNullNativeHandle_ReturnTvgException()
    {
        using var saver = new TvgSaver((_Tvg_Saver*)0);
        using var shape = TvgShape.Create();
        using var animation = TvgAnimation.Create();

        shape.AppendRect(0, 0, 16, 16);
        shape.SetFillColor(255, 0, 0, 255);

        TestApiAssert.AllowsTvgException(() => saver.Save(shape, "missing-output.svg"));
        TestApiAssert.AllowsTvgException(() => saver.Save(animation, "missing-output.gif", quality: 90, fps: 24));
        TestApiAssert.AllowsTvgException(() => saver.Sync());
    }

    [Fact(Skip = "Known hang/flaky when invoking real native saver pipeline; kept for troubleshooting.")]
    public void SaveAndSync_Apis_AreCallable()
    {
        using var saver = TvgSaver.Create();
        using var shape = TvgShape.Create();
        using var animation = TvgAnimation.Create();

        shape.AppendRect(0, 0, 16, 16);
        shape.SetFillColor(255, 0, 0, 255);

        TestApiAssert.AllowsTvgException(() => saver.Save(shape, "missing-output.svg"));
        TestApiAssert.AllowsTvgException(() => saver.Save(animation, "missing-output.gif", quality: 90, fps: 24));
        TestApiAssert.AllowsTvgException(() => saver.Sync());
    }

    [Fact(Skip = "[RISKY-NATIVE] Real native saver path (paint->file) can hang on some environments.")]
    public void SavePaint_ToRealFile_AndSync_AreInvestigative()
    {
        string output = Path.Combine(Path.GetTempPath(), $"thorvg-paint-{Guid.NewGuid():N}.svg");
        try
        {
            using var saver = TvgSaver.Create();
            using var shape = TvgShape.Create();
            shape.AppendRect(0, 0, 32, 32);
            shape.SetFillColor(255, 0, 0, 255);

            TestApiAssert.AllowsTvgException(() => saver.Save(shape, output, 90));
            TestApiAssert.AllowsTvgException(() => saver.Sync());
        }
        finally
        {
            if (File.Exists(output))
                File.Delete(output);
        }
    }

    [Fact(Skip = "[RISKY-NATIVE] Real native saver path (animation->file) can hang on some environments.")]
    public void SaveAnimation_ToRealFile_AndSync_AreInvestigative()
    {
        string output = Path.Combine(Path.GetTempPath(), $"thorvg-anim-{Guid.NewGuid():N}.gif");
        try
        {
            using var saver = TvgSaver.Create();
            using var animation = TvgAnimation.Create();

            TestApiAssert.AllowsTvgException(() => saver.Save(animation, output, quality: 90, fps: 24));
            TestApiAssert.AllowsTvgException(() => saver.Sync());
        }
        finally
        {
            if (File.Exists(output))
                File.Delete(output);
        }
    }
}
