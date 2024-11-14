/*
This file is part of the iText (R) project.
Copyright (c) 1998-2024 Apryse Group NV
Authors: Apryse Software.

This program is offered under a commercial and under the AGPL license.
For commercial licensing, contact us at https://itextpdf.com/sales.  For AGPL licensing, see below.

AGPL licensing:
This program is free software: you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License as published by
the Free Software Foundation, either version 3 of the License, or
(at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU Affero General Public License for more details.

You should have received a copy of the GNU Affero General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/
using System;
using iText.StyledXmlParser;

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

            /// <summary>Alternate glyphs to be used instead of regular grlyphs, e.g. ligatures, Asian scripts, ...</summary>
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
            /// <see cref="iText.Svg.Renderers.Impl.CircleSvgNodeRenderer">circle</see>.
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
            /// <see cref="iText.Svg.Renderers.Impl.EllipseSvgNodeRenderer">ellipse</see>.
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
            /// <see cref="iText.Svg.Renderers.Impl.LineSvgNodeRenderer">line</see>.
            /// </summary>
            /// <since>7.1.2</since>
            public const String LINE = "line";

            /// <summary>
            /// Tag defining a
            /// <see cref="iText.Svg.Renderers.Impl.LinearGradientSvgNodeRenderer">linear gradient</see>.
            /// </summary>
            public const String LINEAR_GRADIENT = "linearGradient";

            /// <summary>Tag defining a link</summary>
            public const String LINK = "link";

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
            /// <see cref="iText.Svg.Renderers.Impl.PathSvgNodeRenderer">path</see>.
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
            /// <see cref="iText.Svg.Renderers.Impl.RectangleSvgNodeRenderer">rectangle</see>.
            /// </summary>
            /// <since>7.1.2</since>
            public const String RECT = "rect";

            /// <summary>Not supported in PDF.</summary>
            public const String SCRIPT = "script";

            /// <summary>Not supported in PDF.</summary>
            public const String SET = "set";

            /// <summary>Tag defining the ramp of colors in a gradient.</summary>
            public const String STOP = "stop";

            /// <summary>Tag defining the color in stop point of a gradient.</summary>
            public const String STOP_COLOR = "stop-color";

            /// <summary>Tag defining the opacity in stop point of a gradient.</summary>
            public const String STOP_OPACITY = "stop-opacity";

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

            /// <summary>Phantom tag for text leaf.</summary>
            public const String TEXT_LEAF = ":text-leaf";

            /// <summary>Tag defining a path on which text can be drawn.</summary>
            public const String TEXT_PATH = "textPath";

            /// <summary>Tag defining the description of an element.</summary>
            /// <remarks>Tag defining the description of an element. Is not rendered.</remarks>
            public const String TITLE = "title";

            /// <summary>Tag defining a span within a text element.</summary>
            public const String TSPAN = "tspan";

            /// <summary>Tag defining the use of a named object.</summary>
            public const String USE = "use";

            /// <summary>Tag defining how to view the image.</summary>
            public const String VIEW = "view";

            /// <summary>Tag defining the vertical kerning values in between two glyphs.</summary>
            public const String VKERN = "vkern";

            /// <summary>Tag defining the xml stylesheet declaration.</summary>
            public const String XML_STYLESHEET = "xml-stylesheet";
        }

        /// <summary>Class containing the constant property names for the attributes of tags in the SVG spec</summary>
        public sealed class Attributes : CommonAttributeConstants {
            /// <summary>Attribute defining the clipping path to be applied to a specific shape or group of shapes.</summary>
            public const String CLIP_PATH = "clip-path";

            /// <summary>Attribute defining the clipping rule in a clipping path (or element thereof).</summary>
            public const String CLIP_RULE = "clip-rule";

            /// <summary>Attribute defining the x value of the center of a circle or ellipse.</summary>
            public const String CX = "cx";

            /// <summary>Attribute defining the y value of the center of a circle or ellipse.</summary>
            public const String CY = "cy";

            /// <summary>Attribute defining the outline of a shape.</summary>
            public const String D = "d";

            /// <summary>Attribute defining the relative x-translation of a text-element</summary>
            public const String DX = "dx";

            /// <summary>Attribute defining the relative y-translation of a text-element</summary>
            public const String DY = "dy";

            /// <summary>Attribute defining the fill color.</summary>
            public const String FILL = "fill";

            /// <summary>Attribute defining the fill opacity.</summary>
            public const String FILL_OPACITY = "fill-opacity";

            /// <summary>Attribute defining the fill rule.</summary>
            public const String FILL_RULE = "fill-rule";

            /// <summary>Attribute defining the font family.</summary>
            public const String FONT_FAMILY = "font-family";

            /// <summary>Attribute defining the font weight.</summary>
            public const String FONT_WEIGHT = "font-weight";

            /// <summary>Attribute defining the font style.</summary>
            public const String FONT_STYLE = "font-style";

            /// <summary>Attribute defining the font size.</summary>
            public const String FONT_SIZE = "font-size";

            /// <summary>The Constant ITALIC.</summary>
            public const String ITALIC = "italic";

            /// <summary>The Constant BOLD.</summary>
            public const String BOLD = "bold";

            /// <summary>Attribute defining the units relation for a color gradient.</summary>
            public const String GRADIENT_UNITS = "gradientUnits";

            /// <summary>Attribute defining the transformations for a color gradient.</summary>
            public const String GRADIENT_TRANSFORM = "gradientTransform";

            /// <summary>Attribute defining the height.</summary>
            /// <remarks>Attribute defining the height. Used in several elements.</remarks>
            public const String HEIGHT = "height";

            /// <summary>Attribute defining the href value.</summary>
            public const String HREF = "href";

            /// <summary>Attribute defining the unique id of an element.</summary>
            public const String ID = "id";

            /// <summary>Attribute defining the marker to use at the end of a path, line, polygon or polyline</summary>
            public const String MARKER_END = "marker-end";

            /// <summary>Attribute defining the height of the viewport in which the marker is to be fitted</summary>
            public const String MARKER_HEIGHT = "markerHeight";

            /// <summary>Attribute defining the marker drawn at every other vertex but the start and end of a path, line, polygon or polyline
            ///     </summary>
            public const String MARKER_MID = "marker-mid";

            /// <summary>Attribute defining the marker to use at the start of a path, line, polygon or polyline</summary>
            public const String MARKER_START = "marker-start";

            /// <summary>Attribute defining the width of the viewport in which the marker is to be fitted</summary>
            public const String MARKER_WIDTH = "markerWidth";

            /// <summary>Attribute defining the coordinate system for attributes ‘markerWidth’, ‘markerHeight’ and the contents of the ‘marker’.
            ///     </summary>
            public const String MARKER_UNITS = "markerUnits";

            /// <summary>Attribute defining the offset of a stop color for gradients.</summary>
            public const String OFFSET = "offset";

            /// <summary>Attribute defining the opacity of a group or graphic element.</summary>
            public const String OPACITY = "opacity";

            /// <summary>Attribute defining the orientation of a marker</summary>
            public const String ORIENT = "orient";

            /// <summary>Close Path Operator.</summary>
            public const String PATH_DATA_CLOSE_PATH = "Z";

            /// <summary>CurveTo Path Operator.</summary>
            public const String PATH_DATA_CURVE_TO = "C";

            /// <summary>Relative CurveTo Path Operator.</summary>
            public const String PATH_DATA_REL_CURVE_TO = "c";

            /// <summary>Attribute defining Elliptical arc path operator.</summary>
            public const String PATH_DATA_ELLIPTICAL_ARC_A = "A";

            /// <summary>Attribute defining Elliptical arc path operator.</summary>
            public const String PATH_DATA_REL_ELLIPTICAL_ARC_A = "a";

            /// <summary>Smooth CurveTo Path Operator.</summary>
            public const String PATH_DATA_CURVE_TO_S = "S";

            /// <summary>Relative Smooth CurveTo Path Operator.</summary>
            public const String PATH_DATA_REL_CURVE_TO_S = "s";

            /// <summary>Absolute LineTo Path Operator.</summary>
            public const String PATH_DATA_LINE_TO = "L";

            /// <summary>Absolute hrizontal LineTo Path Operator.</summary>
            public const String PATH_DATA_LINE_TO_H = "H";

            /// <summary>Relative horizontal LineTo Path Operator.</summary>
            public const String PATH_DATA_REL_LINE_TO_H = "h";

            /// <summary>Absolute vertical LineTo Path operator.</summary>
            public const String PATH_DATA_LINE_TO_V = "V";

            /// <summary>Relative vertical LineTo Path operator.</summary>
            public const String PATH_DATA_REL_LINE_TO_V = "v";

            /// <summary>Relative LineTo Path Operator.</summary>
            public const String PATH_DATA_REL_LINE_TO = "l";

            /// <summary>MoveTo Path Operator.</summary>
            public const String PATH_DATA_MOVE_TO = "M";

            /// <summary>Relative MoveTo Path Operator.</summary>
            public const String PATH_DATA_REL_MOVE_TO = "m";

            /// <summary>Shorthand/smooth quadratic Bézier curveto.</summary>
            public const String PATH_DATA_SHORTHAND_CURVE_TO = "T";

            /// <summary>Relative Shorthand/smooth quadratic Bézier curveto.</summary>
            public const String PATH_DATA_REL_SHORTHAND_CURVE_TO = "t";

            /// <summary>Catmull-Rom curve command.</summary>
            public const String PATH_DATA_CATMULL_CURVE = "R";

            /// <summary>Relative Catmull-Rom curve command.</summary>
            public const String PATH_DATA_REL_CATMULL_CURVE = "r";

            /// <summary>Bearing command.</summary>
            public const String PATH_DATA_BEARING = "B";

            /// <summary>Relative Bearing command.</summary>
            public const String PATH_DATA_REL_BEARING = "b";

            /// <summary>Quadratic CurveTo Path Operator.</summary>
            public const String PATH_DATA_QUAD_CURVE_TO = "Q";

            /// <summary>Relative Quadratic CurveTo Path Operator.</summary>
            public const String PATH_DATA_REL_QUAD_CURVE_TO = "q";

            /// <summary>Attribute defining the coordinate system for the pattern content.</summary>
            public const String PATTERN_CONTENT_UNITS = "patternContentUnits";

            /// <summary>Attribute defining list of transform definitions for the pattern element.</summary>
            public const String PATTERN_TRANSFORM = "patternTransform";

            /// <summary>Attribute defining the coordinate system for attributes x, y, width , and height in pattern.</summary>
            public const String PATTERN_UNITS = "patternUnits";

            /// <summary>Attribute defining the points of a polyline or polygon.</summary>
            public const String POINTS = "points";

            /// <summary>Attribute defining how to preserve the aspect ratio when scaling.</summary>
            public const String PRESERVE_ASPECT_RATIO = "preserveAspectRatio";

            /// <summary>Attribute defining the radius of a circle.</summary>
            public const String R = "r";

            /// <summary>Attribute defining the x-axis coordinate of the reference point which is to be aligned exactly at the marker position.
            ///     </summary>
            public const String REFX = "refX";

            /// <summary>Attribute defining the y-axis coordinate of the reference point which is to be aligned exactly at the marker position.
            ///     </summary>
            public const String REFY = "refY";

            /// <summary>Attribute defining the x-axis of an ellipse or the x-axis radius of rounded rectangles.</summary>
            public const String RX = "rx";

            /// <summary>Attribute defining the y-axis of an ellipse or the y-axis radius of rounded rectangles.</summary>
            public const String RY = "ry";

            /// <summary>Attribute defining the spread method for a color gradient.</summary>
            public const String SPREAD_METHOD = "spreadMethod";

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

            /// <summary>Attribute defining the text anchor used by the text</summary>
            public const String TEXT_ANCHOR = "text-anchor";

            /// <summary>Attribute defining a transformation that needs to be applied.</summary>
            public const String TRANSFORM = "transform";

            /// <summary>Attribute defining the viewbox of an element.</summary>
            public const String VIEWBOX = "viewBox";

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

            /// <summary>Attribute defining XML namespace</summary>
            public const String XMLNS = "xmlns";

            /// <summary>Attribute defining the y value of an element.</summary>
            public const String Y = "y";

            /// <summary>Attribute defining the first y coordinate value of a line.</summary>
            public const String Y1 = "y1";

            /// <summary>Attribute defining the second y coordinate value of a line.</summary>
            public const String Y2 = "y2";

            /// <summary>Attribute defining version</summary>
            public const String VERSION = "version";
        }

        /// <summary>Class containing the constants for values appearing in SVG tags and attributes</summary>
        public sealed class Values {
            /// <summary>Value representing automatic orientation for the marker attribute orient.</summary>
            public const String AUTO = "auto";

            /// <summary>Value representing reverse automatic orientation for the start marker.</summary>
            public const String AUTO_START_REVERSE = "auto-start-reverse";

            /// <summary>Value representing the default value for the stroke linecap.</summary>
            public const String BUTT = "butt";

            /// <summary>Value representing the default aspect ratio: xmidymid.</summary>
            public const String DEFAULT_ASPECT_RATIO = SvgConstants.Values.XMID_YMID;

            /// <summary>Value representing how to preserve the aspect ratio when dealing with images.</summary>
            public const String DEFER = "defer";

            /// <summary>Value representing the fill rule "even odd".</summary>
            public const String FILL_RULE_EVEN_ODD = "evenodd";

            /// <summary>Value representing the fill rule "nonzero".</summary>
            public const String FILL_RULE_NONZERO = "nonzero";

            /// <summary>Value representing the meet for preserve aspect ratio calculations.</summary>
            public const String MEET = "meet";

            /// <summary>Value representing the "none" value".</summary>
            public const String NONE = "none";

            /// <summary>Value representing the units relation "objectBoundingBox".</summary>
            public const String OBJECT_BOUNDING_BOX = "objectBoundingBox";

            /// <summary>The value representing slice for the preserve aspect ratio calculations;</summary>
            public const String SLICE = "slice";

            /// <summary>Value representing the text-alignment end for text objects</summary>
            public const String TEXT_ANCHOR_END = "end";

            /// <summary>Value representing the text-alignment middle for text objects</summary>
            public const String TEXT_ANCHOR_MIDDLE = "middle";

            /// <summary>Value representing the text-alignment start for text objects</summary>
            public const String TEXT_ANCHOR_START = "start";

            /// <summary>Value representing the gradient spread method "pad".</summary>
            public const String SPREAD_METHOD_PAD = "pad";

            /// <summary>Value representing the gradient spread method "repeat".</summary>
            public const String SPREAD_METHOD_REPEAT = "repeat";

            /// <summary>Value representing the gradient spread method "reflect".</summary>
            public const String SPREAD_METHOD_REFLECT = "reflect";

            /// <summary>The value for markerUnits that represent values in a coordinate system which has a single unit equal the size in user units of the current stroke width.
            ///     </summary>
            public const String STROKEWIDTH = "strokeWidth";

            /// <summary>Value representing the units relation "userSpaceOnUse".</summary>
            public const String USER_SPACE_ON_USE = "userSpaceOnUse";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMIN = "xminymin";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMID = "xminymid";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMIN_YMAX = "xminymax";

            /// <summary>Value representing how to align when scaling.</summary>
            public const String XMID_YMID = "xmidymid";

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

            public const String VERSION1_1 = "1.1";
        }
    }
}
