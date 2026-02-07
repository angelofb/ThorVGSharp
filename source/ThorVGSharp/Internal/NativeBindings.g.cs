using System.Runtime.InteropServices;
using ThorVGSharp.Internal.Attributes;

namespace ThorVGSharp.Interop
{
    internal partial struct _Tvg_Canvas
    {
    }

    internal partial struct _Tvg_Paint
    {
    }

    internal partial struct _Tvg_Gradient
    {
    }

    internal partial struct _Tvg_Saver
    {
    }

    internal partial struct _Tvg_Animation
    {
    }

    internal partial struct _Tvg_Accessor
    {
    }

    internal enum Tvg_Result
    {
        TVG_RESULT_SUCCESS = 0,
        TVG_RESULT_INVALID_ARGUMENT,
        TVG_RESULT_INSUFFICIENT_CONDITION,
        TVG_RESULT_FAILED_ALLOCATION,
        TVG_RESULT_MEMORY_CORRUPTION,
        TVG_RESULT_NOT_SUPPORTED,
        TVG_RESULT_UNKNOWN = 255,
    }

    internal enum Tvg_Colorspace
    {
        TVG_COLORSPACE_ABGR8888 = 0,
        TVG_COLORSPACE_ARGB8888,
        TVG_COLORSPACE_ABGR8888S,
        TVG_COLORSPACE_ARGB8888S,
        TVG_COLORSPACE_UNKNOWN = 255,
    }

    internal enum Tvg_Engine_Option
    {
        TVG_ENGINE_OPTION_NONE = 0,
        TVG_ENGINE_OPTION_DEFAULT = 1 << 0,
        TVG_ENGINE_OPTION_SMART_RENDER = 1 << 1,
    }

    internal enum Tvg_Mask_Method
    {
        TVG_MASK_METHOD_NONE = 0,
        TVG_MASK_METHOD_ALPHA,
        TVG_MASK_METHOD_INVERSE_ALPHA,
        TVG_MASK_METHOD_LUMA,
        TVG_MASK_METHOD_INVERSE_LUMA,
        TVG_MASK_METHOD_ADD,
        TVG_MASK_METHOD_SUBTRACT,
        TVG_MASK_METHOD_INTERSECT,
        TVG_MASK_METHOD_DIFFERENCE,
        TVG_MASK_METHOD_LIGHTEN,
        TVG_MASK_METHOD_DARKEN,
    }

    internal enum Tvg_Blend_Method
    {
        TVG_BLEND_METHOD_NORMAL = 0,
        TVG_BLEND_METHOD_MULTIPLY,
        TVG_BLEND_METHOD_SCREEN,
        TVG_BLEND_METHOD_OVERLAY,
        TVG_BLEND_METHOD_DARKEN,
        TVG_BLEND_METHOD_LIGHTEN,
        TVG_BLEND_METHOD_COLORDODGE,
        TVG_BLEND_METHOD_COLORBURN,
        TVG_BLEND_METHOD_HARDLIGHT,
        TVG_BLEND_METHOD_SOFTLIGHT,
        TVG_BLEND_METHOD_DIFFERENCE,
        TVG_BLEND_METHOD_EXCLUSION,
        TVG_BLEND_METHOD_HUE,
        TVG_BLEND_METHOD_SATURATION,
        TVG_BLEND_METHOD_COLOR,
        TVG_BLEND_METHOD_LUMINOSITY,
        TVG_BLEND_METHOD_ADD,
        TVG_BLEND_METHOD_COMPOSITION = 255,
    }

    internal enum Tvg_Type
    {
        TVG_TYPE_UNDEF = 0,
        TVG_TYPE_SHAPE,
        TVG_TYPE_SCENE,
        TVG_TYPE_PICTURE,
        TVG_TYPE_TEXT,
        TVG_TYPE_LINEAR_GRAD = 10,
        TVG_TYPE_RADIAL_GRAD,
    }

    internal enum Tvg_Stroke_Cap
    {
        TVG_STROKE_CAP_BUTT = 0,
        TVG_STROKE_CAP_ROUND,
        TVG_STROKE_CAP_SQUARE,
    }

    internal enum Tvg_Stroke_Join
    {
        TVG_STROKE_JOIN_MITER = 0,
        TVG_STROKE_JOIN_ROUND,
        TVG_STROKE_JOIN_BEVEL,
    }

    internal enum Tvg_Stroke_Fill
    {
        TVG_STROKE_FILL_PAD = 0,
        TVG_STROKE_FILL_REFLECT,
        TVG_STROKE_FILL_REPEAT,
    }

    internal enum Tvg_Fill_Rule
    {
        TVG_FILL_RULE_NON_ZERO = 0,
        TVG_FILL_RULE_EVEN_ODD,
    }

    internal enum Tvg_Text_Wrap
    {
        TVG_TEXT_WRAP_NONE = 0,
        TVG_TEXT_WRAP_CHARACTER,
        TVG_TEXT_WRAP_WORD,
        TVG_TEXT_WRAP_SMART,
        TVG_TEXT_WRAP_ELLIPSIS,
        TVG_TEXT_WRAP_HYPHENATION,
    }

    internal static unsafe partial class NativeMethods
    {
        internal const int TVG_PATH_COMMAND_CLOSE = 0;
        internal const int TVG_PATH_COMMAND_MOVE_TO = 1;
        internal const int TVG_PATH_COMMAND_LINE_TO = 2;
        internal const int TVG_PATH_COMMAND_CUBIC_TO = 3;

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_engine_init([NativeTypeName("unsigned int")] uint threads);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_engine_term();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_engine_version([NativeTypeName("uint32_t *")] uint* major, [NativeTypeName("uint32_t *")] uint* minor, [NativeTypeName("uint32_t *")] uint* micro, [NativeTypeName("const char **")] sbyte** version);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Canvas")]
        public static extern _Tvg_Canvas* tvg_swcanvas_create(Tvg_Engine_Option op);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_swcanvas_set_target([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("uint32_t *")] uint* buffer, [NativeTypeName("uint32_t")] uint stride, [NativeTypeName("uint32_t")] uint w, [NativeTypeName("uint32_t")] uint h, Tvg_Colorspace cs);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Canvas")]
        public static extern _Tvg_Canvas* tvg_glcanvas_create(Tvg_Engine_Option op);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_glcanvas_set_target([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, void* display, void* surface, void* context, [NativeTypeName("int32_t")] int id, [NativeTypeName("uint32_t")] uint w, [NativeTypeName("uint32_t")] uint h, Tvg_Colorspace cs);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Canvas")]
        public static extern _Tvg_Canvas* tvg_wgcanvas_create(Tvg_Engine_Option op);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_wgcanvas_set_target([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, void* device, void* instance, void* target, [NativeTypeName("uint32_t")] uint w, [NativeTypeName("uint32_t")] uint h, Tvg_Colorspace cs, int type);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_destroy([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_add([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_insert([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* target, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* at);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_remove([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_update([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_draw([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("bool")] byte clear);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_sync([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_canvas_set_viewport([NativeTypeName("Tvg_Canvas")] _Tvg_Canvas* canvas, [NativeTypeName("int32_t")] int x, [NativeTypeName("int32_t")] int y, [NativeTypeName("int32_t")] int w, [NativeTypeName("int32_t")] int h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_rel([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort tvg_paint_ref([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort tvg_paint_unref([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("bool")] byte free);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint16_t")]
        public static extern ushort tvg_paint_get_ref([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_visible([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("bool")] byte visible);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte tvg_paint_get_visible([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_scale([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float factor);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_rotate([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float degree);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_translate([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_transform([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const Tvg_Matrix *")] TvgMatrix* m);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_transform([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Matrix *")] TvgMatrix* m);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_opacity([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t")] byte opacity);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_opacity([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t *")] byte* opacity);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_paint_duplicate([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("bool")]
        public static extern byte tvg_paint_intersects([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("int32_t")] int x, [NativeTypeName("int32_t")] int y, [NativeTypeName("int32_t")] int w, [NativeTypeName("int32_t")] int h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_aabb([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float* x, float* y, float* w, float* h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_obb([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Point *")] TvgPoint* pt4);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_mask_method([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* target, Tvg_Mask_Method method);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_mask_method([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const Tvg_Paint")] _Tvg_Paint* target, Tvg_Mask_Method* method);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_clip([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* clipper);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_paint_get_clip([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_paint_get_parent([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_get_type([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, Tvg_Type* type);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_paint_set_blend_method([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, Tvg_Blend_Method method);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_shape_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_reset([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_move_to([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_line_to([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_cubic_to([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float cx1, float cy1, float cx2, float cy2, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_close([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_append_rect([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float x, float y, float w, float h, float rx, float ry, [NativeTypeName("bool")] byte cw);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_append_circle([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float cx, float cy, float rx, float ry, [NativeTypeName("bool")] byte cw);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_append_path([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const Tvg_Path_Command *")] byte* cmds, [NativeTypeName("uint32_t")] uint cmdCnt, [NativeTypeName("const Tvg_Point *")] TvgPoint* pts, [NativeTypeName("uint32_t")] uint ptsCnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_path([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const Tvg_Path_Command **")] byte** cmds, [NativeTypeName("uint32_t *")] uint* cmdsCnt, [NativeTypeName("const Tvg_Point **")] TvgPoint** pts, [NativeTypeName("uint32_t *")] uint* ptsCnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_width([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float width);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_width([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, float* width);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_color([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t")] byte r, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte b, [NativeTypeName("uint8_t")] byte a);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_color([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t *")] byte* r, [NativeTypeName("uint8_t *")] byte* g, [NativeTypeName("uint8_t *")] byte* b, [NativeTypeName("uint8_t *")] byte* a);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_gradient([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_gradient([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Gradient *")] _Tvg_Gradient** grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_dash([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const float *")] float* dashPattern, [NativeTypeName("uint32_t")] uint cnt, float offset);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_dash([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const float **")] float** dashPattern, [NativeTypeName("uint32_t *")] uint* cnt, float* offset);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_cap([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, Tvg_Stroke_Cap cap);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_cap([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, Tvg_Stroke_Cap* cap);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_join([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, Tvg_Stroke_Join join);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_join([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, Tvg_Stroke_Join* join);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_stroke_miterlimit([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float miterlimit);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_stroke_miterlimit([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, float* miterlimit);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_trimpath([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, float begin, float end, [NativeTypeName("bool")] byte simultaneous);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_fill_color([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t")] byte r, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte b, [NativeTypeName("uint8_t")] byte a);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_fill_color([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("uint8_t *")] byte* r, [NativeTypeName("uint8_t *")] byte* g, [NativeTypeName("uint8_t *")] byte* b, [NativeTypeName("uint8_t *")] byte* a);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_fill_rule([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, Tvg_Fill_Rule rule);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_fill_rule([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, Tvg_Fill_Rule* rule);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_paint_order([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("bool")] byte strokeFirst);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_set_gradient([NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_shape_get_gradient([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("Tvg_Gradient *")] _Tvg_Gradient** grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Gradient")]
        public static extern _Tvg_Gradient* tvg_linear_gradient_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Gradient")]
        public static extern _Tvg_Gradient* tvg_radial_gradient_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_linear_gradient_set([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, float x1, float y1, float x2, float y2);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_linear_gradient_get([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, float* x1, float* y1, float* x2, float* y2);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_radial_gradient_set([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, float cx, float cy, float r, float fx, float fy, float fr);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_radial_gradient_get([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, float* cx, float* cy, float* r, float* fx, float* fy, float* fr);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_set_color_stops([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, [NativeTypeName("const Tvg_Color_Stop *")] TvgColorStop* color_stop, [NativeTypeName("uint32_t")] uint cnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_get_color_stops([NativeTypeName("const Tvg_Gradient")] _Tvg_Gradient* grad, [NativeTypeName("const Tvg_Color_Stop **")] TvgColorStop** color_stop, [NativeTypeName("uint32_t *")] uint* cnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_set_spread([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, [NativeTypeName("const Tvg_Stroke_Fill")] Tvg_Stroke_Fill spread);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_get_spread([NativeTypeName("const Tvg_Gradient")] _Tvg_Gradient* grad, Tvg_Stroke_Fill* spread);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_set_transform([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad, [NativeTypeName("const Tvg_Matrix *")] TvgMatrix* m);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_get_transform([NativeTypeName("const Tvg_Gradient")] _Tvg_Gradient* grad, [NativeTypeName("Tvg_Matrix *")] TvgMatrix* m);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_get_type([NativeTypeName("const Tvg_Gradient")] _Tvg_Gradient* grad, Tvg_Type* type);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Gradient")]
        public static extern _Tvg_Gradient* tvg_gradient_duplicate([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_gradient_del([NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* grad);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_picture_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_load([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, [NativeTypeName("const char *")] sbyte* path);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_load_raw([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, [NativeTypeName("const uint32_t *")] uint* data, [NativeTypeName("uint32_t")] uint w, [NativeTypeName("uint32_t")] uint h, Tvg_Colorspace cs, [NativeTypeName("bool")] byte copy);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_load_data([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, [NativeTypeName("const char *")] sbyte* data, [NativeTypeName("uint32_t")] uint size, [NativeTypeName("const char *")] sbyte* mimetype, [NativeTypeName("const char *")] sbyte* rpath, [NativeTypeName("bool")] byte copy);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_set_asset_resolver([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, [NativeTypeName("Tvg_Picture_Asset_Resolver")] delegate* unmanaged[Cdecl]<_Tvg_Paint*, sbyte*, void*, byte> resolver, void* data);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_set_size([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, float w, float h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_get_size([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* picture, float* w, float* h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_set_origin([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_picture_get_origin([NativeTypeName("const Tvg_Paint")] _Tvg_Paint* picture, float* x, float* y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("const Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_picture_get_paint([NativeTypeName("Tvg_Paint")] _Tvg_Paint* picture, [NativeTypeName("uint32_t")] uint id);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_scene_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_insert([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* target, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* at);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_remove([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_clear_effects([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add_effect_gaussian_blur([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, double sigma, int direction, int border, int quality);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add_effect_drop_shadow([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, int r, int g, int b, int a, double angle, double distance, double sigma, int quality);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add_effect_fill([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, int r, int g, int b, int a);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add_effect_tint([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, int black_r, int black_g, int black_b, int white_r, int white_g, int white_b, double intensity);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_scene_add_effect_tritone([NativeTypeName("Tvg_Paint")] _Tvg_Paint* scene, int shadow_r, int shadow_g, int shadow_b, int midtone_r, int midtone_g, int midtone_b, int highlight_r, int highlight_g, int highlight_b, int blend);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_text_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_font([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, [NativeTypeName("const char *")] sbyte* name);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_size([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float size);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_text([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, [NativeTypeName("const char *")] sbyte* utf8);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_align([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float x, float y);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_layout([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float w, float h);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_wrap_mode([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, Tvg_Text_Wrap mode);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_spacing([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float letter, float line);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_italic([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float shear);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_outline([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, float width, [NativeTypeName("uint8_t")] byte r, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte b);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_color([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, [NativeTypeName("uint8_t")] byte r, [NativeTypeName("uint8_t")] byte g, [NativeTypeName("uint8_t")] byte b);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_text_set_gradient([NativeTypeName("Tvg_Paint")] _Tvg_Paint* text, [NativeTypeName("Tvg_Gradient")] _Tvg_Gradient* gradient);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_font_load([NativeTypeName("const char *")] sbyte* path);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_font_load_data([NativeTypeName("const char *")] sbyte* name, [NativeTypeName("const char *")] sbyte* data, [NativeTypeName("uint32_t")] uint size, [NativeTypeName("const char *")] sbyte* mimetype, [NativeTypeName("bool")] byte copy);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_font_unload([NativeTypeName("const char *")] sbyte* path);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Saver")]
        public static extern _Tvg_Saver* tvg_saver_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_saver_save_paint([NativeTypeName("Tvg_Saver")] _Tvg_Saver* saver, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("uint32_t")] uint quality);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_saver_save_animation([NativeTypeName("Tvg_Saver")] _Tvg_Saver* saver, [NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("const char *")] sbyte* path, [NativeTypeName("uint32_t")] uint quality, [NativeTypeName("uint32_t")] uint fps);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_saver_sync([NativeTypeName("Tvg_Saver")] _Tvg_Saver* saver);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_saver_del([NativeTypeName("Tvg_Saver")] _Tvg_Saver* saver);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Animation")]
        public static extern _Tvg_Animation* tvg_animation_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_set_frame([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float no);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Paint")]
        public static extern _Tvg_Paint* tvg_animation_get_picture([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_get_frame([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float* no);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_get_total_frame([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float* cnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_get_duration([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float* duration);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_set_segment([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float begin, float end);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_get_segment([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float* begin, float* end);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_animation_del([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Accessor")]
        public static extern _Tvg_Accessor* tvg_accessor_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_accessor_del([NativeTypeName("Tvg_Accessor")] _Tvg_Accessor* accessor);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_accessor_set([NativeTypeName("Tvg_Accessor")] _Tvg_Accessor* accessor, [NativeTypeName("Tvg_Paint")] _Tvg_Paint* paint, [NativeTypeName("bool (*)(Tvg_Paint, void *)")] delegate* unmanaged[Cdecl]<_Tvg_Paint*, void*, byte> func, void* data);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint tvg_accessor_generate_id([NativeTypeName("const char *")] sbyte* name);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("Tvg_Animation")]
        public static extern _Tvg_Animation* tvg_lottie_animation_new();

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: NativeTypeName("uint32_t")]
        public static extern uint tvg_lottie_animation_gen_slot([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("const char *")] sbyte* slot);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_apply_slot([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("uint32_t")] uint id);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_del_slot([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("uint32_t")] uint id);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_set_marker([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("const char *")] sbyte* marker);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_get_markers_cnt([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("uint32_t *")] uint* cnt);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_get_marker([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("uint32_t")] uint idx, [NativeTypeName("const char **")] sbyte** name);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_tween([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, float from, float to, float progress);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_assign([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("const char *")] sbyte* layer, [NativeTypeName("uint32_t")] uint ix, [NativeTypeName("const char *")] sbyte* var, float val);

        [DllImport("thorvg", CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern Tvg_Result tvg_lottie_animation_set_quality([NativeTypeName("Tvg_Animation")] _Tvg_Animation* animation, [NativeTypeName("uint8_t")] byte value);

        [NativeTypeName("#define TVG_VERSION_MAJOR 1")]
        public const int TVG_VERSION_MAJOR = 1;

        [NativeTypeName("#define TVG_VERSION_MINOR 0")]
        public const int TVG_VERSION_MINOR = 0;

        [NativeTypeName("#define TVG_VERSION_MICRO 0")]
        public const int TVG_VERSION_MICRO = 0;
    }
}
