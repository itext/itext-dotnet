using System;
using System.Collections.Generic;
using iText.IO.Util;

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
            public const String CIRCLE = "circle";

            public const String DEFS = "defs";

            public const String ELLIPSE = "ellipse";

            public const String FOREIGN_OBJECT = "foreignObject";

            public const String D = "d";

            public const String G = "g";

            public const String IMAGE = "image";

            public const String LINE = "line";

            public const String LINEAR_GRADIENT = "linearGradient";

            public const String PATH = "path";

            public const String PATTERN = "pattern";

            public const String POLYLINE = "polyline";

            public const String POLYGON = "polygon";

            public const String RADIAL_GRADIENT = "radialGradient";

            public const String RECT = "rect";

            public const String SVG = "svg";

            public const String SYMBOL = "symbol";

            public const String TEXT = "text";

            public const String TSPAN = "tspan";

            public const String TEXTPATH = "textpath";

            public const String USE = "use";

            public const String STYLE = "style";
        }

        /// <summary>Class containing the constant property names for the attributes of tags in the SVG spec</summary>
        public sealed class Attributes {
            public const String X = "x";

            public const String Y = "y";

            public const String CX = "cx";

            public const String CY = "cy";

            public const String R = "r";

            public const String RX = "rx";

            public const String RY = "ry";

            public const String WIDTH = "width";

            public const String HEIGHT = "height";

            public const String TRANSFORM = "transform";

            public const String VIEWBOX = "viewbox";

            public const String X1 = "x1";

            public const String X2 = "x2";

            public const String Y1 = "y1";

            public const String Y2 = "y2";

            public const String POINTS = "points";

            public const String PRESERVE_ASPECT_RATIO = "preserveaspectratio";

            public const String STROKE = "stroke";

            public const String FILL = "fill";

            public const String STROKE_WIDTH = "stroke-width";

            public const String FILL_RULE_EVEN_ODD = "evenodd";

            public const String FILL_RULE = "fill-rule";

            public const String FONT_SIZE = "font-size";

            public const String ID = "id";

            public const String TEXT_CONTENT = "text_content";

            public const String PATH_DATA_ELIP_ARC = "A";

            public const String PATH_DATA_ELIP_ARC_RELATIVE = "a";

            public const String PATH_DATA_LINE_TO = "L";

            public const String PATH_DATA_LINE_RELATIVE_TO = "l";

            public const String PATH_DATA_MOVE_TO = "M";

            public const String PATH_DATA_MOVE_RELATIVE_TO = "m";

            public const String PATH_DATA_HORIZNTL_TO = "H";

            public const String PATH_DATA_HORIZNTL_RELATIVE_TO = "h";

            public const String PATH_DATA_VERTICL_TO = "V";

            public const String PATH_DATA_VERTICL_RELATIVE_TO = "v";

            public const String PATH_DATA_CLOSE_PATH = "z";

            public const String PATH_DATA_CURVE_TO = "C";

            public const String PATH_DATA_CURVE_RELATIVE_TO = "c";

            public const String PATH_DATA_CURVE_TO_S = "S";

            public const String PATH_DATA_CURVE_TO_RELATIVE_S = "s";

            public const String PATH_DATA_QUARD_CURVE_TO = "Q";

            public const String PATH_DATA_QUARD_CURVE_RELATIVE_TO = "q";

            public const String PATH_DATA_QUARD_CURVE_TO_T = "T";

            public const String PATH_DATA_QUARD_CURVE_TO_RELATIVE_T = "t";

            public const String ANIMATE = "animate";

            public const String ANIMATE_MOTION = "animateMotion";

            public const String ANIMATE_TRANSFORM = "animateTransform";

            public const String DISCARD = "discard";

            public const String SET = "set";

            public static readonly ICollection<String> ANIMATION_ELEMENTS = JavaCollectionsUtil.UnmodifiableSet(new HashSet
                <String>(JavaUtil.ArraysAsList(ANIMATE, ANIMATE_MOTION, ANIMATE_TRANSFORM, DISCARD, SET)));

            public const String STYLE = "style";
            //Viewbox, Position & Dimension
            //Stroke and fill
            //Text and font
            //Svg path element commands
            //Animation
            //CSS
        }

        /// <summary>Class containing the constants for values appearing in SVG tags and attributes</summary>
        public sealed class Values {
            public const String DEFAULT_ASPECT_RATIO = "xmidymid";

            public const String DEFER = "defer";

            public const String MEET = "meet";

            public const String MEET_OR_SLICE_DEFAULT = "meet";

            public const String NONE = "none";

            public const String SLICE = "slice";

            public const String XMIN_YMIN = "xminymin";

            public const String XMIN_YMID = "xminymid";

            public const String XMIN_YMAX = "xminymax";

            public const String XMID_YMIN = "xmidymin";

            public const String XMID_YMID = "xmidymid";

            public const String XMID_YMAX = "xmidymax";

            public const String XMAX_YMIN = "xmaxymin";

            public const String XMAX_YMID = "xmaxymid";

            public const String XMAX_YMAX = "xmaxymax";
            // values
        }
    }
}
