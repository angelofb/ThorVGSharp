using Silk.NET.SDL;
using System.Runtime.InteropServices;
using ThorVGSharp;
using ThorVGSharp.Sample.Showcase.Examples;

namespace ThorVGSharp.Sample.Showcase;

class Program
{
    const int DEFAULT_WIDTH = 1024;
    const int DEFAULT_HEIGHT = 1024;

    static unsafe Window* window;
    static TvgCanvasSoftware? canvas;
    static Sdl sdl = null!;
    static bool running = true;

    // All available examples
    static readonly (string Name, Func<Example> Factory, int Width, int Height)[] Examples =
    [
        ("Animation", () => new AnimationExample(), 1024, 1024),
        ("Blending", () => new BlendingExample(), 1800, 1380),
        ("BoundingBox", () => new BoundingBoxExample(), 900, 900)
    ];

    static int currentExampleIndex = 0;
    static Example? currentExample;
    static uint[]? buffer;
    static GCHandle bufferHandle;
    static int bufferWidth;
    static int bufferHeight;

    static string resourcePath = "external/thorvg.example/res";

    static unsafe void Main(string[] args)
    {
        try
        {
            // Parse command line arguments for resource path
            if (args.Length > 0)
            {
                resourcePath = args[0];
            }

            if (!Directory.Exists(resourcePath))
            {
                Console.WriteLine($"ERROR: Resource directory not found: {resourcePath}");
                Console.WriteLine($"Usage: ThorVGSharp.Sample.Showcase [resource-path]");
                Console.WriteLine($"Example: ThorVGSharp.Sample.Showcase external/thorvg.example/res");
                return;
            }

            Console.WriteLine($"Using resources from: {resourcePath}");
            Console.WriteLine();

            Example.ResourceBasePath = resourcePath;

            sdl = Sdl.GetApi();

            if (sdl.Init(Sdl.InitVideo) < 0)
            {
                Console.WriteLine($"SDL init failed: {sdl.GetErrorS()}");
                return;
            }

            TvgEngine.Initialize(4);

            // Create window
            window = sdl.CreateWindow(
                "ThorVG Showcase - Press 1-3 to switch examples, ESC to quit",
                Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                DEFAULT_WIDTH, DEFAULT_HEIGHT,
                (uint)(WindowFlags.Shown | WindowFlags.Resizable)
            );

            if (window == null)
            {
                Console.WriteLine($"Window creation failed: {sdl.GetErrorS()}");
                return;
            }

            // Load first example
            LoadExample(0);

            Console.WriteLine("ThorVG Showcase");
            Console.WriteLine("===============");
            for (int i = 0; i < Examples.Length; i++)
            {
                Console.WriteLine($"  {i + 1}. {Examples[i].Name}");
            }
            Console.WriteLine("\nPress 1-3 to switch examples, ESC to quit");

            // Main loop
            var lastTime = sdl.GetTicks();

            while (running)
            {
                HandleEvents();

                if (!running) break;

                var currentTime = sdl.GetTicks();
                var elapsed = currentTime - lastTime;
                lastTime = currentTime;

                if (currentExample != null)
                {
                    // Update example
                    if (currentExample.Update(canvas!, currentExample.Elapsed))
                    {
                        Render();
                    }

                    currentExample.Elapsed += elapsed;
                }

                sdl.Delay(1);
            }

            // Cleanup
            currentExample?.Dispose();
            canvas?.Dispose();

            if (bufferHandle.IsAllocated)
                bufferHandle.Free();

            sdl.DestroyWindow(window);

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            try { TvgEngine.Terminate(); } catch { }
            sdl.Quit();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nFATAL ERROR: {ex.GetType().Name}");
            Console.WriteLine($"Message: {ex.Message}");
            Console.WriteLine($"Stack trace:\n{ex.StackTrace}");

            try
            {
                currentExample?.Dispose();
                canvas?.Dispose();
                if (window != null) sdl?.DestroyWindow(window);
                try { TvgEngine.Terminate(); } catch { }
                sdl?.Quit();
            }
            catch { }
        }
    }

    static unsafe void LoadExample(int index)
    {
        if (index < 0 || index >= Examples.Length) return;

        currentExampleIndex = index;
        var (name, factory, width, height) = Examples[index];

        Console.WriteLine($"\nLoading example: {name}");

        // Dispose previous example
        currentExample?.Dispose();
        canvas?.Dispose();
        if (bufferHandle.IsAllocated)
            bufferHandle.Free();

        // Resize window
        sdl.SetWindowSize(window, width, height);
        sdl.SetWindowTitle(window, $"ThorVG Showcase - {name}");

        // Create new buffer
        buffer = new uint[width * height];
        bufferWidth = width;
        bufferHeight = height;
        bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
        var bufferPtr = bufferHandle.AddrOfPinnedObject();

        // Create canvas
        canvas = TvgCanvasSoftware.Create();
        canvas.SetTarget(bufferPtr, (uint)width, (uint)width, (uint)height, TvgColorSpace.Argb8888);

        // Create and load example
        currentExample = factory();
        if (!currentExample.Content(canvas, (uint)width, (uint)height))
        {
            Console.WriteLine($"Failed to load example: {name}");
            return;
        }

        currentExample.Elapsed = 0;

        // Initial render
        Render();
    }

    static unsafe void Render()
    {
        if (canvas == null || buffer == null) return;

        canvas.Update();
        canvas.Draw(true);
        canvas.Sync();

        // Copy buffer to window
        var surface = sdl.GetWindowSurface(window);
        if (surface == null) return;

        var dstWidth = surface->W;
        var dstHeight = surface->H;
        var dstPitch = surface->Pitch / 4; // pitch in pixels (divide by 4 for uint)

        var minWidth = Math.Min(bufferWidth, dstWidth);
        var minHeight = Math.Min(bufferHeight, dstHeight);

        var destPtr = (uint*)surface->Pixels;

        // Copy line by line respecting pitch
        for (int y = 0; y < minHeight; y++)
        {
            var srcOffset = y * bufferWidth;
            var dstOffset = y * dstPitch;

            var sourceLine = buffer.AsSpan(srcOffset, minWidth);
            var destLine = new Span<uint>(destPtr + dstOffset, minWidth);

            sourceLine.CopyTo(destLine);
        }

        sdl.UpdateWindowSurface(window);
    }

    static void HandleEvents()
    {
        Event evt = default;
        while (sdl.PollEvent(ref evt) != 0)
        {
            switch ((EventType)evt.Type)
            {
                case EventType.Quit:
                    running = false;
                    break;

                case EventType.Keydown:
                    var key = (KeyCode)evt.Key.Keysym.Sym;
                    switch (key)
                    {
                        case KeyCode.KEscape:
                            running = false;
                            break;
                        case KeyCode.K1:
                            LoadExample(0);
                            break;
                        case KeyCode.K2:
                            LoadExample(1);
                            break;
                        case KeyCode.K3:
                            LoadExample(2);
                            break;
                    }
                    break;

                case EventType.Mousebuttondown:
                    currentExample?.ClickDown(canvas!, evt.Button.X, evt.Button.Y);
                    break;

                case EventType.Mousebuttonup:
                    currentExample?.ClickUp(canvas!, evt.Button.X, evt.Button.Y);
                    break;

                case EventType.Mousemotion:
                    currentExample?.Motion(canvas!, evt.Motion.X, evt.Motion.Y);
                    break;

                case EventType.Windowevent:
                    if (evt.Window.Event == (byte)WindowEventID.Resized)
                    {
                        LoadExample(currentExampleIndex); // Reload on resize
                    }
                    break;
            }
        }
    }
}
