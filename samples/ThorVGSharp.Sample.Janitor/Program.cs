using Silk.NET.SDL;
using System.Runtime.InteropServices;
using ThorVGSharp;

namespace ThorVGSharp.Sample.Janitor;

unsafe class Program
{
    const int BASE_WIDTH = 3840;
    const int BASE_HEIGHT = 2160;
    const float SCALE = 0.5333333333333f;
    static readonly int SCREEN_WIDTH = (int)(BASE_WIDTH * SCALE);
    static readonly int SCREEN_HEIGHT = (int)(BASE_HEIGHT * SCALE);

    static Window* window;
    static TvgCanvasSoftware? canvas;
    static ThorJanitorGame? game;
    static Sdl sdl = null!;
    static bool running = true;

    static void Main(string[] args)
    {
        try
        {
            sdl = Sdl.GetApi();

            if (sdl.Init(Sdl.InitVideo) < 0)
            {
                Console.WriteLine($"SDL init failed: {sdl.GetErrorS()}");
                return;
            }

            TvgEngine.Initialize(4);

            // Create window WITHOUT SDL_Renderer (we use GetWindowSurface instead)
            window = sdl.CreateWindow(
                "Thor Janitor - Clean the Galaxy!",
                Sdl.WindowposUndefined, Sdl.WindowposUndefined,
                SCREEN_WIDTH, SCREEN_HEIGHT,
                (uint)WindowFlags.Shown
            );

            if (window == null)
            {
                Console.WriteLine($"Window creation failed: {sdl.GetErrorS()}");
                return;
            }

            // Allocate pixel buffer and pin it so the pointer stays valid
            var buffer = new uint[SCREEN_WIDTH * SCREEN_HEIGHT];
            var bufferHandle = GCHandle.Alloc(buffer, GCHandleType.Pinned);
            var bufferPtr = bufferHandle.AddrOfPinnedObject();

            // Create ThorVG software canvas and set target once
            canvas = TvgCanvasSoftware.Create();
            canvas.SetTarget(bufferPtr, (uint)SCREEN_WIDTH, (uint)SCREEN_WIDTH, (uint)SCREEN_HEIGHT, TvgColorSpace.Argb8888);

            // Create game (adds paint objects to canvas)
            game = new ThorJanitorGame(canvas, SCREEN_WIDTH, SCREEN_HEIGHT, SCALE);

            // Initial render
            canvas.Update();
            canvas.Draw(true);
            canvas.Sync();
            CopyBufferToWindow(buffer);

            // Show window
            sdl.ShowWindow(window);

            Console.WriteLine("Game ready! Arrow keys to move, A to shoot, ESC to quit.");

            var lastTime = sdl.GetTicks();

            // Main game loop
            while (running)
            {
                HandleEvents();

                var currentTime = sdl.GetTicks();
                var elapsed = currentTime - lastTime;
                lastTime = currentTime;

                // Get keyboard state
                int numKeys;
                byte* keys = sdl.GetKeyboardState(&numKeys);

                // Update game logic
                game.Update(elapsed, keys);

                // Render frame
                canvas.Update();
                canvas.Draw(false);
                canvas.Sync();

                // Copy to SDL window
                CopyBufferToWindow(buffer);

                sdl.Delay(1);
            }

            // Cleanup
            game.Dispose();
            game = null;
            canvas.Dispose();
            canvas = null;
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
                game?.Dispose();
                canvas?.Dispose();
                if (window != null) sdl?.DestroyWindow(window);
                try { TvgEngine.Terminate(); } catch { }
                sdl?.Quit();
            }
            catch { }
        }
    }

    static void HandleEvents()
    {
        Event evt;
        while (sdl.PollEvent(&evt) != 0)
        {
            switch ((EventType)evt.Type)
            {
                case EventType.Quit:
                    running = false;
                    break;
                case EventType.Keydown:
                    if (evt.Key.Keysym.Sym == (int)KeyCode.KEscape)
                        running = false;
                    break;
            }
        }
    }

    static void CopyBufferToWindow(uint[] buffer)
    {
        var surface = sdl.GetWindowSurface(window);
        if (surface == null) return;

        fixed (uint* src = buffer)
        {
            Buffer.MemoryCopy(src, surface->Pixels,
                (long)surface->W * surface->H * 4,
                buffer.Length * sizeof(uint));
        }
        sdl.UpdateWindowSurface(window);
    }
}
