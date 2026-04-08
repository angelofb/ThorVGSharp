using System.Runtime.InteropServices;

namespace ThorVGSharp.Tests;

[Collection("TvgEngine")]
public class TvgFontManagerTests : IDisposable
{
    public TvgFontManagerTests()
    {
        TvgEngine.Initialize();
    }

    public void Dispose()
    {
        TvgEngine.Terminate();
    }

    [Fact]
    public void LoadData_ManagedOverload_ValidatesInputs()
    {
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("", new byte[] { 1, 2, 3 }));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", ReadOnlySpan<byte>.Empty));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", new byte[] { 1, 2, 3 }, copy: false));
    }

    [Fact]
    public void LoadData_PointerOverload_ValidatesInputs()
    {
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("", IntPtr.Zero, 0));
        Assert.Throws<ArgumentException>(() => TvgFontManager.LoadData("MyFont", IntPtr.Zero, 12));
    }

    [Fact]
    public void LoadAndUnload_WithInvalidPaths_ReturnTvgException()
    {
        TestApiAssert.AllowsTvgException(() => TvgFontManager.Load("missing-font.ttf"));
        TestApiAssert.AllowsTvgException(() => TvgFontManager.Load(new string('a', 600) + ".ttf"));
        TestApiAssert.AllowsTvgException(() => TvgFontManager.Unload("missing-font.ttf"));
        TestApiAssert.AllowsTvgException(() => TvgFontManager.Unload(new string('b', 600) + ".ttf"));
    }

    [Fact]
    public void LoadData_Overloads_ExerciseEncodingBranches()
    {
        byte[] managedData = [0, 1, 2, 3];
        TestApiAssert.AllowsTvgException(() => TvgFontManager.LoadData("ManagedFont", managedData, "font/ttf", copy: true));
        TestApiAssert.AllowsTvgException(() => TvgFontManager.LoadData(new string('f', 600), managedData, new string('m', 600), copy: true));

        IntPtr unmanaged = Marshal.AllocHGlobal(managedData.Length);
        try
        {
            Marshal.Copy(managedData, 0, unmanaged, managedData.Length);
            TestApiAssert.AllowsTvgException(() => TvgFontManager.LoadData("PtrFont", unmanaged, (uint)managedData.Length, null, copy: false));
            TestApiAssert.AllowsTvgException(() => TvgFontManager.LoadData(new string('p', 600), unmanaged, (uint)managedData.Length, new string('t', 600), copy: true));
        }
        finally
        {
            Marshal.FreeHGlobal(unmanaged);
        }
    }
}
