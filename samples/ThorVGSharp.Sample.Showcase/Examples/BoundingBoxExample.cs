using ThorVGSharp;

namespace ThorVGSharp.Sample.Showcase.Examples;

/// <summary>
/// BoundingBox Example - Demonstrates AABB and OBB bounding boxes for various shapes
/// Ported from: thorvg.example/src/BoundingBox.cpp
/// </summary>
internal class BoundingBoxExample : Example
{
    private readonly List<TvgPaint> _paints = new();

    public override bool Content(TvgCanvas canvas, uint width, uint height)
    {
        // Load font
        var fontData = LoadResource("font/PublicSans-Regular.ttf");
        TvgFontManager.LoadData("PublicSans-Regular", fontData, "font/ttf");

        // Circle with ellipse transformation
        {
            var shape = TvgShape.Create();
            shape.AppendCircle(50, 100, 40, 100);
            shape.SetFillColor(0, 30, 255);
            canvas.Add(shape);
            _paints.Add(shape);
            DrawBoundingBox(canvas, shape);
        }

        // Rotated text
        {
            var text = TvgText.Create();
            text.SetFont("PublicSans-Regular");
            text.SetSize(30);
            text.SetText("Text Test");
            text.SetColor(255, 255, 0);
            text.Translate(100, 20);
            text.Rotate(16.0f);
            canvas.Add(text);
            _paints.Add(text);
            DrawBoundingBox(canvas, text);
        }

        // Rotated rectangle
        {
            var shape = TvgShape.Create();
            shape.AppendRect(200, 30, 100, 20);
            shape.SetFillColor(200, 150, 55);
            shape.Rotate(30);
            canvas.Add(shape);
            _paints.Add(shape);
            DrawBoundingBox(canvas, shape);
        }

        // Complex transformed shape
        {
            var shape = TvgShape.Create();
            shape.AppendRect(225, -50, 75, 50, 20, 25);
            shape.AppendCircle(225, 25, 50, 25);
            shape.SetStrokeWidth(10);
            shape.SetStrokeColor(255, 255, 255);
            shape.SetFillColor(50, 50, 155);

            var matrix = new TvgMatrix(1.732f, -1.0f, 30.0f, 1.0f, 1.732f, -70.0f, 0, 0, 1);
            shape.SetTransform(matrix);

            canvas.Add(shape);
            _paints.Add(shape);
            DrawBoundingBox(canvas, shape);
        }

        // SVG picture
        {
            var svgData = LoadResource("svg/tiger.svg");
            var svg = TvgPicture.Create();
            svg.LoadData(svgData, "svg");
            svg.Scale(0.3f);
            svg.Translate(620, 50);
            canvas.Add(svg);
            _paints.Add(svg);
            DrawBoundingBox(canvas, svg);
        }

        // Rotated SVG
        {
            var svgData = LoadResource("svg/tiger.svg");
            var svg = TvgPicture.Create();
            svg.LoadData(svgData, "svg");
            svg.Scale(0.2f);
            svg.Translate(140, 215);
            svg.Rotate(45);
            canvas.Add(svg);
            _paints.Add(svg);
            DrawBoundingBox(canvas, svg);
        }

        // Scene with PNG
        {
            var pngData = LoadResource("image/test.png");
            var scene = TvgScene.Create();
            scene.Scale(0.3f);
            scene.Translate(280, 330);

            var img = TvgPicture.Create();
            img.LoadData(pngData, "png");
            scene.Add(img);
            // Note: img is owned by scene, don't add to _paints

            canvas.Add(scene);
            _paints.Add(scene);
            DrawBoundingBox(canvas, scene);
        }

        // Rotated scene with JPG
        {
            var jpgData = LoadResource("image/test.jpg");
            var scene = TvgScene.Create();
            scene.Scale(0.3f);
            scene.Rotate(80);
            scene.Translate(200, 480);

            var img = TvgPicture.Create();
            img.LoadData(jpgData, "jpg");
            scene.Add(img);
            // Note: img is owned by scene, don't add to _paints

            canvas.Add(scene);
            _paints.Add(scene);
            DrawBoundingBox(canvas, scene);
        }

        // Stroke line
        {
            var line = TvgShape.Create();
            line.MoveTo(470, 350);
            line.LineTo(770, 350);
            line.SetStrokeWidth(20);
            line.SetStrokeColor(55, 55, 0);
            canvas.Add(line);
            _paints.Add(line);
            DrawBoundingBox(canvas, line);
        }

        // Bezier curve
        {
            var curve = TvgShape.Create();
            curve.MoveTo(0, 0);
            curve.CubicTo(40.0f, -10.0f, 120.0f, -150.0f, 80.0f, 0.0f);
            curve.Translate(50, 770);
            curve.SetStrokeWidth(2.0f);
            curve.SetStrokeColor(255, 255, 255);
            canvas.Add(curve);
            _paints.Add(curve);
            DrawBoundingBox(canvas, curve);
        }

        // Rotated bezier curve
        {
            var curve = TvgShape.Create();
            curve.MoveTo(0, 0);
            curve.CubicTo(40.0f, -10.0f, 120.0f, -150.0f, 80.0f, 0.0f);
            curve.Translate(150, 750);
            curve.Rotate(20.0f);
            curve.SetStrokeWidth(2.0f);
            curve.SetStrokeColor(255, 0, 255);
            canvas.Add(curve);
            _paints.Add(curve);
            DrawBoundingBox(canvas, curve);
        }

        // Scene with rotated triangle
        {
            var scene = TvgScene.Create();
            scene.Translate(550, 370);
            scene.Scale(0.7f);

            var shape = TvgShape.Create();
            shape.MoveTo(0, 0);
            shape.LineTo(300, 200);
            shape.LineTo(0, 200);
            shape.SetFillColor(255, 0, 0);
            shape.Close();
            shape.Rotate(20);
            scene.Add(shape);
            // Note: shape is owned by scene, don't add to _paints

            canvas.Add(scene);
            _paints.Add(scene);
            DrawBoundingBox(canvas, scene);
        }

        return true;
    }

    private void DrawBoundingBox(TvgCanvas canvas, TvgPaint paint)
    {
        // Update paint to get accurate bounds
        canvas.Update();

        // Draw AABB (Axis-Aligned Bounding Box) - solid red line
        var (x, y, w, h) = paint.GetBounds();
        var aabb = TvgShape.Create();
        aabb.MoveTo(x, y);
        aabb.LineTo(x + w, y);
        aabb.LineTo(x + w, y + h);
        aabb.LineTo(x, y + h);
        aabb.Close();
        aabb.SetStrokeWidth(2.0f);
        aabb.SetStrokeColor(255, 0, 0, 255);
        canvas.Add(aabb);
        _paints.Add(aabb);

        // Draw OBB (Oriented Bounding Box) - dashed white line
        var points = paint.GetOrientedBounds();
        var obb = TvgShape.Create();
        obb.MoveTo(points[0].X, points[0].Y);
        obb.LineTo(points[1].X, points[1].Y);
        obb.LineTo(points[2].X, points[2].Y);
        obb.LineTo(points[3].X, points[3].Y);
        obb.Close();
        obb.SetStrokeWidth(2.0f);

        ReadOnlySpan<float> dash = stackalloc float[] { 3.0f, 10.0f };
        obb.SetStrokeDash(dash);
        obb.SetStrokeColor(255, 255, 255, 255);

        canvas.Add(obb);
        _paints.Add(obb);
    }

    public override void Dispose()
    {
        foreach (var paint in _paints)
            paint.Dispose();

        _paints.Clear();

        base.Dispose();
    }
}
