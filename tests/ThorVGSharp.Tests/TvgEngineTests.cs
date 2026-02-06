// Tests adapted from external/thorvg/test/testInitializer.cpp
namespace ThorVGSharp.Tests;

public class TvgEngineTests : IDisposable
{
    public TvgEngineTests()
    {
        // Ensure clean state before each test
        if (TvgEngine.IsInitialized)
            TvgEngine.Terminate();
    }

    public void Dispose()
    {
        // Clean up after each test
        if (TvgEngine.IsInitialized)
            TvgEngine.Terminate();
    }

    [Fact]
    public void BasicInitialization()
    {
        TvgEngine.Initialize();
        Assert.True(TvgEngine.IsInitialized);

        TvgEngine.Terminate();
        Assert.False(TvgEngine.IsInitialized);
    }

    [Fact]
    public void MultipleInitialization()
    {
        TvgEngine.Initialize();
        Assert.True(TvgEngine.IsInitialized);

        // Should not throw on multiple initializations
        TvgEngine.Initialize();
        Assert.True(TvgEngine.IsInitialized);

        TvgEngine.Terminate();
        Assert.False(TvgEngine.IsInitialized);

        TvgEngine.Initialize();
        Assert.True(TvgEngine.IsInitialized);

        TvgEngine.Terminate();
        Assert.False(TvgEngine.IsInitialized);

        // Should not throw on multiple terminations
        TvgEngine.Terminate();
        Assert.False(TvgEngine.IsInitialized);
    }

    [Fact]
    public void VersionInformation()
    {
        var version = TvgEngine.Version;
        Assert.NotNull(version);
        Assert.NotEmpty(version);

        var (major, minor, micro) = TvgEngine.GetVersion();
        Assert.True(major >= 0);
        Assert.True(minor >= 0);
        Assert.True(micro >= 0);

        // Version string should match numeric version
        var expectedVersion = $"{major}.{minor}.{micro}";
        Assert.Equal(expectedVersion, version);
    }

    [Fact]
    public void InitializationWithThreadCount()
    {
        TvgEngine.Initialize(4);
        Assert.True(TvgEngine.IsInitialized);
        Assert.Equal(4u, TvgEngine.ThreadCount);

        TvgEngine.Terminate();
        Assert.False(TvgEngine.IsInitialized);
        Assert.Equal(0u, TvgEngine.ThreadCount);
    }
}
