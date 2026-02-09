# ThorVGSharp Showcase

Interactive showcase of ThorVG examples ported from the official ThorVG C++ examples.

## Overview

This project demonstrates various ThorVG features by porting the original C++ examples to C# using ThorVGSharp bindings. It provides an interactive window where you can switch between different examples using keyboard shortcuts.

## Features

- **Interactive Navigation**: Switch between examples using number keys (1-3)
- **External Resources**: Loads resources directly from ThorVG example directory
- **Safe Silk.NET APIs**: Uses safe Span-based APIs where possible
- **Cross-Platform**: Runs on Windows, macOS, and Linux

## Running the Showcase

From the repository root:

```bash
dotnet run --project samples/ThorVGSharp.Sample.Showcase/ThorVGSharp.Sample.Showcase.csproj -- external/thorvg.example/res
```

Or from the Showcase directory:

```bash
cd samples/ThorVGSharp.Sample.Showcase
dotnet run ../../external/thorvg.example/res
```

The argument specifies the path to the ThorVG example resources directory (defaults to `external/thorvg.example/res`).

## Controls

- **1** - Animation example (Lottie playback)
- **2** - Blending example (17 blend modes)
- **3** - BoundingBox example (AABB and OBB visualization)
- **ESC** - Quit

## Current Examples

### 1. Animation
Demonstrates Lottie animation playback with frame-by-frame control.

- Loads Lottie JSON data
- Scales animation to fit window
- Animated playback with progress calculation

**Source:** `external/thorvg.example/src/Animation.cpp`

### 2. Blending
Showcases all 17 blend modes supported by ThorVG.

- Solid colors (opaque and semi-transparent)
- Linear gradients
- Raw pixel data (ARGB8888)
- SVG graphics

**Blend modes demonstrated:**
- Normal, Multiply, Screen, Overlay
- Darken, Lighten, ColorDodge, ColorBurn
- HardLight, SoftLight, Difference, Exclusion
- Hue, Saturation, Color, Luminosity, Add

**Source:** `external/thorvg.example/src/Blending.cpp`

### 3. BoundingBox
Visualizes axis-aligned (AABB) and oriented (OBB) bounding boxes for various paint objects.

- Shapes (circles, rectangles, paths)
- Text with transformations
- SVG pictures
- Scenes with images
- Bezier curves

**Source:** `external/thorvg.example/src/BoundingBox.cpp`

## Project Structure

```
ThorVGSharp.Sample.Showcase/
├── Program.cs                          # Main application loop
├── ExampleFramework.cs                 # Base class for examples
├── Examples/
│   ├── AnimationExample.cs            # Lottie animation
│   ├── BlendingExample.cs             # Blend modes
│   └── BoundingBoxExample.cs          # Bounding box visualization
├── SAMPLES_PORTED.md                  # Porting status tracker
└── README.md                          # This file

Resources are loaded from external/thorvg.example/res/:
├── font/                              # TrueType fonts
├── image/                             # PNG, JPG, raw images
├── lottie/                            # Lottie animations (100+)
└── svg/                               # SVG graphics (80+)
```

## Architecture

### Example Base Class

All examples inherit from `Example` and implement:

- `Content(canvas, width, height)` - Initial scene setup (required)
- `Update(canvas, elapsed)` - Frame updates for animation (optional)
- `ClickDown/Up/Motion(canvas, x, y)` - Mouse interaction (optional)

### Resource Loading

Resources are loaded directly from the filesystem using the provided resource base path:

```csharp
var fontData = LoadResource("font/PublicSans-Regular.ttf");
TvgFontManager.LoadData("PublicSans-Regular", fontData, "font/ttf");
```

The `LoadResource()` method combines the base path with the relative path and reads the file.

### Window Management

- SDL2 via Silk.NET bindings
- Software canvas rendering (TvgCanvasSoftware)
- ARGB8888 pixel format
- Resizable window support

## Adding New Examples

1. Create a new class in `Examples/` inheriting from `Example`
2. Implement `Content()` method with your ThorVG scene
3. Optionally implement `Update()` for animation
4. Add to `Examples` array in `Program.cs`:

```csharp
("MyExample", () => new MyExample(), width, height)
```

5. Update `SAMPLES_PORTED.md` with porting status

See `SAMPLES_PORTED.md` for a list of examples ready to port.

## Dependencies

- **ThorVGSharp** - .NET bindings for ThorVG
- **Silk.NET.SDL** - SDL2 bindings for window/input management
- **.NET 10.0** - Target framework

## Resources

All resources are loaded from `external/thorvg.example/res/`:

- **Fonts**: `font/PublicSans-Regular.ttf`
- **Images**: test.png, test.jpg, rawimage_200x300.raw
- **Lottie**: sample.json and 106+ other animations
- **SVG**: tiger.svg and 82+ other vector graphics

Resources are not embedded in the assembly to save space - they're loaded at runtime from the ThorVG examples directory.

## Known Limitations

- Software rendering only (no OpenGL/WebGPU backends yet)
- Resize reloads the current example
- Some advanced ThorVG features not yet ported

## Contributing

When porting new examples:

1. Match the C++ example functionality as closely as possible
2. Use safe APIs (ReadOnlySpan, etc.) where available
3. Follow C# naming conventions (PascalCase)
4. Add comments explaining ThorVG features
5. Update `SAMPLES_PORTED.md` with status

## License

MIT License - Same as ThorVGSharp and ThorVG

## References

- [ThorVG Project](https://github.com/thorvg/thorvg)
- [ThorVG Examples](https://github.com/thorvg/thorvg/tree/main/examples)
- [ThorVGSharp](https://github.com/angelofb/ThorVGSharp)
