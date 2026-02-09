# ThorVG Examples Porting Status

This document tracks the porting status of ThorVG C++ examples to ThorVGSharp.

## Ported Examples

| # | Example Name | Source File | Status | Notes |
|---|--------------|-------------|--------|-------|
| 1 | Animation | `Animation.cpp` | ✅ Complete | Lottie animation playback with progress control |
| 2 | Blending | `Blending.cpp` | ✅ Complete | All 17 blend modes demonstrated with shapes, gradients, images, and SVG |
| 3 | BoundingBox | `BoundingBox.cpp` | ✅ Complete | AABB and OBB visualization for shapes, text, pictures, and scenes |

## Resources Copied

All resources from `external/thorvg.example/res/` have been copied to `Resources/`:

- ✅ `font/` - TrueType fonts for text rendering
- ✅ `image/` - PNG, JPG, and raw image files
- ✅ `lottie/` - Lottie JSON animation files
- ✅ `svg/` - SVG vector graphics files

## Pending Examples

The following examples from ThorVG are candidates for porting:

### Basic Features
- [ ] **Shapes** - Basic shape rendering (rectangles, circles, paths)
- [ ] **Path** - Path construction and manipulation
- [ ] **Stroke** - Stroke styling (width, caps, joins)
- [ ] **StrokeLine** - Line stroke variations
- [ ] **FillRule** - Fill rule demonstrations (even-odd, non-zero)
- [ ] **Transform** - 2D transformations (translate, rotate, scale, matrix)

### Advanced Features
- [ ] **Scene** - Scene hierarchy and composition
- [ ] **Clipping** - Clipping path demonstrations
- [ ] **Masking** - Alpha masking techniques
- [ ] **MaskingMethods** - Different masking methods
- [ ] **Opacity** - Opacity and transparency
- [ ] **Duplicate** - Paint duplication and cloning

### Gradients
- [ ] **LinearGradient** - Linear gradient fills
- [ ] **RadialGradient** - Radial gradient fills
- [ ] **GradientStroke** - Gradient strokes
- [ ] **GradientTransform** - Gradient transformations
- [ ] **GradientMasking** - Gradient-based masking
- [ ] **FillSpread** - Fill spread modes

### Images
- [ ] **PicturePng** - PNG image loading
- [ ] **PictureJpg** - JPEG image loading
- [ ] **PictureSvg** - SVG loading
- [ ] **PictureWebp** - WebP image loading
- [ ] **PictureRaw** - Raw pixel data
- [ ] **ImageRotation** - Image rotation
- [ ] **ImageScaling** - Image scaling

### Text
- [ ] **Text** - Basic text rendering
- [ ] **TextLayout** - Text layout and alignment
- [ ] **TextLineWrap** - Text wrapping
- [ ] **TextEffects** - Text effects (shadows, etc.)

### Animation & Lottie
- [ ] **Lottie** - Advanced Lottie features
- [ ] **LottieExpressions** - Lottie expressions
- [ ] **LottieInteraction** - Interactive Lottie
- [ ] **LottieTweening** - Lottie tweening
- [ ] **LottieExtension** - Lottie extensions

### Scene Composition
- [ ] **SceneBlending** - Scene-level blending
- [ ] **SceneEffects** - Scene effects
- [ ] **SceneTransform** - Scene transformations

### Effects
- [ ] **EffectDropShadow** - Drop shadow effects

### Advanced Topics
- [ ] **Accessor** - Direct paint property access
- [ ] **DirectUpdate** - Direct paint updates
- [ ] **Retaining** - Paint retaining mode
- [ ] **Viewport** - Viewport transformations
- [ ] **Particles** - Particle system
- [ ] **Intersects** - Intersection detection
- [ ] **TrimPath** - Path trimming
- [ ] **MultiCanvas** - Multiple canvas usage
- [ ] **CustomTransform** - Custom transformations
- [ ] **DataLoad** - Data loading techniques
- [ ] **Svg** - Advanced SVG features
- [ ] **GifSaver** - GIF export

### Stress Tests
- [ ] **Stress** - Performance stress tests

### C API
- [ ] **Capi** - C API demonstrations

## Implementation Notes

### Differences from C++ Version

1. **Resource Loading**: Using embedded resources instead of file paths
2. **Memory Management**: Using C# garbage collection with GCHandle pinning where needed
3. **API Naming**: Following C# conventions (PascalCase, properties instead of getters)
4. **Safety**: Using ReadOnlySpan<T> and safe APIs where possible
5. **Error Handling**: Using exceptions instead of return codes where appropriate

### Framework Differences

- **Example Base Class**: Simplified from C++ template to C# abstract class
- **Window Management**: Using Silk.NET SDL2 bindings
- **Canvas**: Software rendering only (SwCanvas equivalent)
- **Resource Access**: Reflection-based embedded resource loading

## How to Add New Examples

1. Create new file in `Examples/` directory (e.g., `ShapesExample.cs`)
2. Inherit from `Example` base class
3. Implement `Content()` method (required)
4. Optionally implement `Update()`, `ClickDown()`, `ClickUp()`, `Motion()`
5. Add to `Examples` array in `Program.cs`
6. Update this document with porting status
7. Copy any required resources to `Resources/` directory

## Testing

Run the showcase:
```bash
dotnet run --project samples/ThorVGSharp.Sample.Showcase/ThorVGSharp.Sample.Showcase.csproj
```

Switch between examples using number keys (1-3).
