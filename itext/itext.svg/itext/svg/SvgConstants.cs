using System;

namespace iText.Svg {
    /// <summary>
    /// A class containing constant values signifying the proeprty names of tags, attribute, CSS-style
    /// and certain values in SVG XML.
    /// </summary>
    public sealed class SvgConstants {
        private SvgConstants() {
        }

        /// <summary>Class containing the constant property names for the tags in the SVG spec</summary>
        public sealed class Tags {
            /// <summary>Tag defining a Hyperlink.</summary>
            public const String A = "a";

            /// <summary>Alternate glyphs to be used instead of regular grlyphs, e.g.</summary>
            /// <remarks>Alternate glyphs to be used instead of regular grlyphs, e.g. ligatures, Asian scripts, ...</remarks>
            public const String ALT_GLYPH = "altGlyph";

            /// <summary>Defines a set of glyph substitions.</summary>
            public const String ALT_GLYPH_DEF = "altGlyphDef";

            /// <summary>Defines a candidate set of glyph substitutions.</summary>
            public const String ALT_GLYPH_ITEM = "altGlyphItem";

            /// <summary>Not supported in PDF.</summary>
            public const String ANIMATE = "animate";

            /// <summary>Not supported in PDF.</summary>
            public const String ANIMATE_MOTION = "animateMotion";

            /// <summary>Not supported in PDF.</summary>
            public const String ANIMATE_COLOR = "animateColor";

            /// <summary>Not supported in PDF.</summary>
            public const String ANIMATE_TRANSFORM = "animateTransform";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.CircleSvgNodeRenderer">circle</see>
            /// .
            /// </summary>
            /// <since>7.1.2</since>
            public const String CIRCLE = "circle";

            /// <summary>Tag defining a clipping path.</summary>
            /// <remarks>Tag defining a clipping path. A clipping path defines the region where can be drawn. Anything outside the path won't be drawn.
            ///     </remarks>
            public const String CLIP_PATH = "clipPath";

            /// <summary>Tag defining the color profile to be used.</summary>
            public const String COLOR_PROFILE = "color-profile";

            /// <summary>Not supported in PDF</summary>
            public const String CURSOR = "cursor";

            /// <summary>Tag defining objects that can be reused from another context</summary>
            public const String DEFS = "defs";

            /// <summary>Tag defining the description of its parent element</summary>
            public const String DESC = "desc";

            /// <summary>
            /// Tag defining an
            /// <see cref="iText.Svg.Renderers.Impl.EllipseSvgNodeRenderer">ellipse</see>
            /// .
            /// </summary>
            /// <since>7.1.2</since>
            public const String ELLIPSE = "ellipse";

            /// <summary>Tag defining how to blend two objects together.</summary>
            public const String FE_BLEND = "feBlend";

            /// <summary>Tag defining the color matrix transformations that can be performed.</summary>
            public const String FE_COLOR_MATRIX = "feColorMatrix";

            /// <summary>Tag defining color component remapping.</summary>
            public const String FE_COMPONENT_TRANSFER = "feComponentTransfer";

            /// <summary>Tag defining the combination of two input images.</summary>
            public const String FE_COMPOSITE = "feComposite";

            /// <summary>Tag defining a matrix convolution filter</summary>
            public const String FE_COMVOLVE_MATRIX = "feConvolveMatrix";

            /// <summary>Tag defining the lighting map.</summary>
            public const String FE_DIFFUSE_LIGHTING = "feDiffuseLighting";

            /// <summary>Tag defining the values to displace an image.</summary>
            public const String FE_DISPLACEMENT_MAP = "feDisplacementMap";

            /// <summary>Tag defining a distant light source.</summary>
            public const String FE_DISTANT_LIGHT = "feDistantLight";

            /// <summary>Tag defining the fill of a subregion.</summary>
            public const String FE_FLOOD = "feFlood";

            /// <summary>Tag defining the transfer function for the Alpha component.</summary>
            public const String FE_FUNC_A = "feFuncA";

            /// <summary>Tag defining the transfer function for the Blue component.</summary>
            public const String FE_FUNC_B = "feFuncB";

            /// <summary>Tag defining the transfer function for the Green component.</summary>
            public const String FE_FUNC_G = "feFuncG";

            /// <summary>Tag defining the transfer function for the Red component.</summary>
            public const String FE_FUNC_R = "feFuncR";

            /// <summary>Tag defining the blur values.</summary>
            public const String FE_GAUSSIAN_BLUR = "feGaussianBlur";

            /// <summary>Tag defining a image data from a source.</summary>
            public const String FE_IMAGE = "feImage";

            /// <summary>Tag defining that filters will be applied concurrently instead of sequentially.</summary>
            public const String FE_MERGE = "feMerge";

            /// <summary>Tag defining a node in a merge.</summary>
            public const String FE_MERGE_NODE = "feMergeNode";

            /// <summary>Tag defining the erosion or dilation of an image.</summary>
            public const String FE_MORPHOLOGY = "feMorphology";

            /// <summary>Tag defining the offset of an image.</summary>
            public const String FE_OFFSET = "feOffset";

            /// <summary>Tag defining a point light effect.</summary>
            public const String FE_POINT_LIGHT = "fePointLight";

            /// <summary>Tag defining a lighting map.</summary>
            public const String FE_SPECULAR_LIGHTING = "feSpecularLighting";

            /// <summary>Tag defining a spotlight.</summary>
            public const String FE_SPOTLIGHT = "feSpotLight";

            /// <summary>Tag defining a fill that can be repeated.</summary>
            /// <remarks>Tag defining a fill that can be repeated. Similar to PATTERN.</remarks>
            public const String FE_TILE = "feTile";

            /// <summary>Tag defining values for the perlin turbulence function.</summary>
            public const String FE_TURBULENCE = "feTurbulence";

            /// <summary>Tag defining a collection of filter operations.</summary>
            public const String FILTER = "filter";

            /// <summary>Tag defining a font.</summary>
            public const String FONT = "font";

            /// <summary>Tag defining a font-face.</summary>
            public const String FONT_FACE = "font-face";

            /// <summary>Tag defining the formats of the font.</summary>
            public const String FONT_FACE_FORMAT = "font-face-format";

            /// <summary>Tag defining the name of the font.</summary>
            public const String FONT_FACE_NAME = "font-face-name";

            /// <summary>Tag defining the source file of the font.</summary>
            public const String FONT_FACE_SRC = "font-face-src";

            /// <summary>Tag defining the URI of a font.</summary>
            public const String FONT_FACE_URI = "font-face-uri";

            /// <summary>Tag definign a foreign XML standard to be inserted.</summary>
            /// <remarks>Tag definign a foreign XML standard to be inserted. E.g. MathML</remarks>
            public const String FOREIGN_OBJECT = "foreignObject";

            /// <summary>Tag defining a group of elements.</summary>
            public const String G = "g";

            /// <summary>Tag defining a single glyph.</summary>
            public const String GLYPH = "glyph";

            /// <summary>Tag defining a sigle glyph for altGlyph.</summary>
            public const String GLYPH_REF = "glyphRef";

            /// <summary>Tag defining the horizontal kerning values in between two glyphs.</summary>
            public const String HKERN = "hkern";

            /// <summary>Tag defining an image.</summary>
            public const String IMAGE = "image";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.LineSvgNodeRenderer">line</see>
            /// .
            /// </summary>
            /// <since>7.1.2</since>
            public const String LINE = "line";

            /// <summary>Tag defining a linear gradient</summary>
            public const String LINEAR_GRADIENT = "linearGradient";

            /// <summary>Tag defining the graphics (arrowheads or polymarkers) to be drawn at the end of paths, lines, etc.
            ///     </summary>
            public const String MARKER = "marker";

            /// <summary>Tag defining a mask.</summary>
            public const String MASK = "mask";

            /// <summary>Tag defining metadata.</summary>
            public const String METADATA = "metadata";

            /// <summary>Tag defining content to be rendered if a glyph is missing from the font.</summary>
            public const String MISSING_GLYPH = "missing-glyph";

            /// <summary>Not supported in PDF</summary>
            public const String MPATH = "mpath";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.PathSvgNodeRenderer">path</see>
            /// .
            /// </summary>
            /// <since>7.1.2</since>
            public const String PATH = "path";

            /// <summary>Tag defining a graphical object that can be repeated.</summary>
            public const String PATTERN = "pattern";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.PolygonSvgNodeRenderer">polygon</see>
            /// shape.
            /// </summary>
            /// <since>7.1.2</since>
            public const String POLYGON = "polygon";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.PolylineSvgNodeRenderer">polyline</see>
            /// shape.
            /// </summary>
            /// <since>7.1.2</since>
            public const String POLYLINE = "polyline";

            /// <summary>Tag defining a radial gradient</summary>
            public const String RADIAL_GRADIENT = "radialGradient";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.RectangleSvgNodeRenderer">rectangle</see>
            /// .
            /// </summary>
            /// <since>7.1.2</since>
            public const String RECT = "rect";

            /// <summary>Not supported in PDF.</summary>
            public const String SCRIPT = "script";

            /// <summary>Not supported in PDF.</summary>
            public const String SET = "set";

            /// <summary>Tag defining the ramp of colors in a gradient.</summary>
            public const String STOP = "stop";

            /// <summary>Tag defining the style to be.</summary>
            public const String STYLE = "style";

            /// <summary>
            /// Tag defining an
            /// <see cref="iText.Svg.Renderers.Impl.SvgTagSvgNodeRenderer">SVG</see>
            /// element.
            /// </summary>
            /// <since>7.1.2</since>
            public const String SVG = "svg";

            /// <summary>Tag defining a switch element.</summary>
            public const String SWITCH = "switch";

            /// <summary>Tag defining graphical templates that can be reused by the use tag.</summary>
            public const String SYMBOL = "symbol";

            /// <summary>Tag defining text to be drawn on a page/screen.</summary>
            /// <since>7.1.2</since>
            public const String TEXT = "text";

            /// <summary>Tag defining a path on which text can be drawn.</summary>
            public const String TEXT_PATH = "textPath";

            /// <summary>Tag defining the description of an element.</summary>
            /// <remarks>Tag defining the description of an element. Is not rendered.</remarks>
            public const String TITLE = "title";

            /// <summary>Deprecated in SVG.</summary>
            /// <remarks>Deprecated in SVG. Tag defining text that was defined in an SVG document.</remarks>
            public const String TREF = "tref";

            /// <summary>Tag defining a span within a text element.</summary>
            public const String TSPAN = "tspan";

            /// <summary>Tag defining the use of a named object.</summary>
            public const String USE = "use";

            /// <summary>Tag defining how to view the image.</summary>
            public const String VIEW = "view";

            /// <summary>Tag defining the vertical kerning values in between two glyphs.</summary>
            public const String VKERN = "vkern";
        }

        /// <summary>Class containing the constant property names for the attributes of tags in the SVG spec</summary>
        public sealed class Attributes {
            /// <summary>Attribute defining the x value of the center of a circle or ellipse.</summary>
            public const String CX = "cx";

            /// <summary>Attribute defining the y value of the center of a circle or ellipse.</summary>
            public const String CY = "cy";

            /// <summary>Attribute defining the outline of a shape.</summary>
            public const String D = "d";

            /// <summary>Attribute defining the fill color.</summary>
            public const String FILL = "fill";

            /// <summary>Attribute defining the fill opacity.</summary>
            public const String FILL_OPACITY = "fill-opacity";

            /// <summary>Attribute defining the fill rule.</summary>
            public const String FILL_RULE = "fill-rule";

            /// <summary>Attribute defining the font family.</summary>
            public const String FONT_FAMILY = "font-family";

            /// <summary>Attribute defining the font size.</summary>
            public const String FONT_SIZE = "font-size";

            /// <summary>Attribute defining the height.</summary>
            /// <remarks>Attribute defining the height. Used in several elements.</remarks>
            public const String HEIGHT = "height";

            /// <summary>Attribute defining the href value.</summary>
            public const String HREF = "href";

            /// <summary>Attribute defining the unique id of an element.</summary>
            public const String ID = "id";

            /// <summary>Attribute defining the radius of a circle.</summary>
            public const String R = "r";

            /// <summary>Attribute defining the x-axis of an ellipse or the x-axis radius of rounded rectangles.</summary>
            public const String RX = "rx";

            /// <summary>Attribute defining the y-axis of an ellipse or the y-axis radius of rounded rectangles.</summary>
            public const String RY = "ry";

            /// <summary>Colse Path Operator.</summary>
            public const String PATH_DATA_CLOSE_PATH = "z";

            /// <summary>CurveTo Path Operator.</summary>
            public const String PATH_DATA_CURVE_TO = "C";

            /// <summary>Smooth CurveTo Path Operator.</summary>
            public const String PATH_DATA_CURVE_TO_S = "S";

            /// <summary>LineTo Path Operator.</summary>
            public const String PATH_DATA_LINE_TO = "L";

            /// <summary>MoveTo Path Operator.</summary>
            public const String PATH_DATA_MOVE_TO = "M";

            /// <summary>Quadratic CurveTo Path Operator.</summary>
            public const String PATH_DATA_QUAD_CURVE_TO = "Q";

            /// <summary>Attribute defining the points of a polyline or polygon.</summary>
            public const String POINTS = "points";

            /// <summary>Attribute defining how to preserve the aspect ratio when scaling.</summary>
            public const String PRESERVE_ASPECT_RATIO = "preserveaspectratio";

            /// <summary>Attribute defining the stroke color.</summary>
            public const String STROKE = "stroke";

            /// <summary>Attribute defining the stroke dash offset.</summary>
            public const String STROKE_DASHARRAY = "stroke-dasharray";

            /// <summary>Attribute defining the stroke dash offset.</summary>
            public const String STROKE_DASHOFFSET = "stroke-dashoffset";

            /// <summary>Attribute defining the stroke linecap.</summary>
            public const String STROKE_LINECAP = "stroke-linecap";

            /// <summary>Attribute defining the stroke miterlimit.</summary>
            public const String STROKE_MITERLIMIT = "stroke-miterlimit";

            /// <summary>Attribute defingin the stroke opacity.</summary>
            public const String STROKE_OPACITY = "stroke-opacity";

            /// <summary>Attribute defining the stroke width.</summary>
            public const String STROKE_WIDTH = "stroke-width";

            /// <summary>Attribute defining the style of an element.</summary>
            public const String STYLE = "style";

            /// <summary>Attribute defining the text content of a text node.</summary>
            public const String TEXT_CONTENT = "text_content";

            /// <summary>Attribute defining a transformation that needs to be applied.</summary>
            public const String TRANSFORM = "transform";

            /// <summary>Attribute defining the viewbox of an element.</summary>
            public const String VIEWBOX = "viewbox";

            /// <summary>Attribute defining the width of an element.</summary>
            public const String WIDTH = "width";

            /// <summary>Attribute defining the x value of an element.</summary>
            public const String X = "x";

            /// <summary>Attribute defining the first x coordinate value of a line.</summary>
            public const String X1 = "x1";

            /// <summary>Attribute defining the second x coordinate value of a line.</summary>
            public const String X2 = "x2";

            /// <summary>Attribute defining image source.</summary>
            public const String XLINK_HREF = "xlink:href";

            /// <summary>Attribute defining the y value of an element.</summary>
            public const String Y = "y";

            /// <summary>Attribute defining the first y coordinate value of a line.</summary>
            public const String Y1 = "y1";

            /// <summary>Attribute defining the second y coordinate value of a line.</summary>
            public const String Y2 = "y2";
        }

        /// <summary>Class containing the constants for values appearing in SVG tags and attributes</summary>
        public sealed class Values {
            /// <summary>Value representing the default value for the stroke linecap.</summary>
            public const String BUTT = "butt";

            /// <summary>Value representing the default aspect ratio: xmidymid.</summary>
            public const String DEFAULT_ASPECT_RATIO = "xmidymid";

            /// <summary>Value representing how to preserve the aspect ratio when dealing with images.</summary>
            public const String DEFER = "defer";

            /// <summary>Value representing the fill rule "even odd".</summary>
            public const String FILL_RULE_EVEN_ODD = "evenodd";

            /// <summary>Value representing the fill rule "nonzero".</summary>
            public const String FILL_RULE_NONZERO = "nonzero";

            /// <summary>Value representing the "none" value".</summary>
            public const String NONE = "none";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMIN = "xminymin";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMID = "xminymid";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMAX = "xminymax";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMID_YMIN = "xmidymin";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMID_YMAX = "xmidymax";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMAX_YMIN = "xmaxymin";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMAX_YMID = "xmaxymid";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMAX_YMAX = "xmaxymax";
        }
    }
}
