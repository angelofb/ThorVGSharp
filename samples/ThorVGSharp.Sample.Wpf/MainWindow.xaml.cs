using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

using ThorVGSharp.Sample.Common;

namespace ThorVGSharp.Sample.Wpf;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        TvgEngine.Initialize((uint)Environment.ProcessorCount);
        //TvgInitializer.Init(1);

        InitializeComponent();

        Closing += MainWindow_Closing;
    }

    private void MainWindow_Closing(object? sender, CancelEventArgs e)
    {
        TvgEngine.Terminate();
    }
}

public class RenderSurface : FrameworkElement
{
    private WriteableBitmap? _bitmap;
    private TvgCanvasSoftware? _canvas;
    private TvgScene? _scene;

    public RenderSurface()
    {
        SizeChanged += RenderSurface_SizeChanged;
        Loaded += (s, e) => InitialRender();
    }

    private void InitialRender()
    {
        if (ActualWidth > 0 && ActualHeight > 0)
        {
            PerformResize((int)ActualWidth, (int)ActualHeight);
        }
    }

    private void RenderSurface_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        var width = (int)ActualWidth;
        var height = (int)ActualHeight;

        if (width <= 0 || height <= 0)
            return;

        PerformResize(width, height);
    }

    private void PerformResize(int width, int height)
    {
        var sw = Stopwatch.StartNew();

        // Create or recreate bitmap
        _bitmap = new WriteableBitmap(
            width, height,
            96, 96,
            PixelFormats.Bgra32,
            null);

        // IMPORTANT: Stride is in BYTES, not pixels
        // For BGRA32, each pixel is 4 bytes
        var stride = _bitmap.BackBufferStride;

        _bitmap.Lock();

        try
        {
            // Reuse canvas if possible, otherwise create new one
            if (_canvas == null)
            {
                _canvas = TvgCanvasSoftware.Create(TvgEngineOption.SmartRender);
                if (_canvas == null)
                {
                    Console.WriteLine("[ERROR] Failed to create canvas");
                    return;
                }
            }

            // Remove old scene if exists
            if (_scene != null)
            {
                _canvas.Remove(_scene);
                _scene.Dispose();
            }

            // Create a new scene (fast operation)
            _scene = TvgScene.Create();
            if (_scene == null)
            {
                Console.WriteLine("[ERROR] Failed to create scene");
                return;
            }

            // Add scene to canvas
            _canvas.Add(_scene);

            // Populate scene with content
            Samples.DrawThorToScene(_scene, width, height);

            try
            {
                // Update canvas target to new buffer
                _canvas.SetTarget(_bitmap.BackBuffer, (uint)stride / 4, (uint)width, (uint)height, TvgColorSpace.Argb8888);

                // Update canvas for new size
                _canvas.Update();

                // Draw
                _canvas.Draw(true);

                // Wait for completion
                _canvas.Sync();
            }
            catch (TvgException ex)
            {
                Console.WriteLine($"[ERROR] ThorVG operation failed: {ex.Message}");
                return;
            }

            _bitmap.AddDirtyRect(new Int32Rect(0, 0, width, height));
        }
        finally
        {
            _bitmap.Unlock();
        }

        InvalidateVisual();

        sw.Stop();
        Console.WriteLine($"Elapsed {sw.ElapsedMilliseconds} ms");
    }

    protected override void OnRender(DrawingContext drawingContext)
    {
        if (_bitmap != null)
        {
            drawingContext.DrawImage(_bitmap, new Rect(0, 0, ActualWidth, ActualHeight));
        }
    }

    // Cleanup
    ~RenderSurface()
    {
        _scene?.Dispose();
        _canvas?.Dispose();
    }
}
