/*

This file is part of the iText (R) project.
Copyright (c) 1998-2022 iText Group NV
Authors: Bruno Lowagie, Paulo Soares, et al.

This program is free software; you can redistribute it and/or modify
it under the terms of the GNU Affero General Public License version 3
as published by the Free Software Foundation with the addition of the
following permission added to Section 15 as permitted in Section 7(a):
FOR ANY PART OF THE COVERED WORK IN WHICH THE COPYRIGHT IS OWNED BY
ITEXT GROUP. ITEXT GROUP DISCLAIMS THE WARRANTY OF NON INFRINGEMENT
OF THIRD PARTY RIGHTS

This program is distributed in the hope that it will be useful, but
WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
or FITNESS FOR A PARTICULAR PURPOSE.
See the GNU Affero General Public License for more details.
You should have received a copy of the GNU Affero General Public License
along with this program; if not, see http://www.gnu.org/licenses or write to
the Free Software Foundation, Inc., 51 Franklin Street, Fifth Floor,
Boston, MA, 02110-1301 USA, or download the license from the following URL:
http://itextpdf.com/terms-of-use/

The interactive user interfaces in modified source and object code versions
of this program must display Appropriate Legal Notices, as required under
Section 5 of the GNU Affero General Public License.

In accordance with Section 7(b) of the GNU Affero General Public License,
a covered work must retain the producer line in every PDF that is created
or manipulated using iText.

You can be released from the requirements of the license by purchasing
a commercial license. Buying such a license is mandatory as soon as you
develop commercial activities involving the iText software without
disclosing the source code of your own applications.
These activities include: offering paid services to customers as an ASP,
serving PDFs on the fly in a web application, shipping iText with a closed
source product.

For more information, please contact iText Software Corp. at this
address: sales@itextpdf.com
*/
namespace iText.Kernel.Pdf.Canvas {
    /// <summary>A container for constants defined in the PDF specification (ISO 32000-1).</summary>
    public class PdfCanvasConstants {
        private PdfCanvasConstants() {
        }

        // This private constructor will prevent the instantiation of this class
        /// <summary>
        /// The text rendering mode determines whether showing text causes glyph
        /// outlines to be stroked, filled, used as a clipping boundary, or some
        /// combination of the three.
        /// </summary>
        /// <remarks>
        /// The text rendering mode determines whether showing text causes glyph
        /// outlines to be stroked, filled, used as a clipping boundary, or some
        /// combination of the three. Stroking, filling, and clipping have the same
        /// effects for a text object as they do for a path object, although they are
        /// specified in an entirely different way.
        /// If the text rendering mode calls for filling, the current nonstroking
        /// color in the graphics state is used; if it calls for stroking, the
        /// current stroking color is used.
        /// All documentation for this class is taken from ISO 32000-1, section 9.3.6
        /// "Text Rendering Mode".
        /// </remarks>
        public sealed class TextRenderingMode {
            private TextRenderingMode() {
            }

            /// <summary>Fill text</summary>
            public const int FILL = 0;

            /// <summary>Stroke text, providing the outline of the glyphs</summary>
            public const int STROKE = 1;

            /// <summary>Fill and stroke text</summary>
            public const int FILL_STROKE = 2;

            /// <summary>Neither fill nor stroke, i.e. render invisibly</summary>
            public const int INVISIBLE = 3;

            /// <summary>Fill text and add to path for clipping</summary>
            public const int FILL_CLIP = 4;

            /// <summary>Stroke text and add to path for clipping</summary>
            public const int STROKE_CLIP = 5;

            /// <summary>Fill, then stroke text and add to path for clipping</summary>
            public const int FILL_STROKE_CLIP = 6;

            /// <summary>Add text to path for clipping</summary>
            public const int CLIP = 7;
        }

        /// <summary>
        /// The line cap style specifies the shape to be used at the ends of open
        /// subpaths (and dashes, if any) when they are stroked.
        /// </summary>
        /// <remarks>
        /// The line cap style specifies the shape to be used at the ends of open
        /// subpaths (and dashes, if any) when they are stroked.
        /// All documentation for this class is taken from ISO 32000-1, section
        /// 8.4.3.3 "Line Cap Style".
        /// </remarks>
        public class LineCapStyle {
            private LineCapStyle() {
            }

            // This private constructor will prevent the instantiation of this class
            /// <summary>The stroke is squared of at the endpoint of the path.</summary>
            /// <remarks>
            /// The stroke is squared of at the endpoint of the path. There is no
            /// projection beyond the end of the path.
            /// </remarks>
            public const int BUTT = 0;

            /// <summary>
            /// A semicircular arc with a diameter equal to the line width is drawn
            /// around the endpoint and filled in.
            /// </summary>
            public const int ROUND = 1;

            /// <summary>
            /// The stroke continues beyond the endpoint of the path for a distance
            /// equal to half the line width and is squared off.
            /// </summary>
            public const int PROJECTING_SQUARE = 2;
        }

        /// <summary>
        /// The line join style specifies the shape to be used at the corners of
        /// paths that are stroked.
        /// </summary>
        /// <remarks>
        /// The line join style specifies the shape to be used at the corners of
        /// paths that are stroked. Join styles are significant only at points where
        /// consecutive segments of a path connect at an angle; segments that meet or
        /// intersect fortuitously receive no special treatment.
        /// All documentation for this class is taken from ISO 32000-1, section
        /// 8.4.3.4 "Line Join Style".
        /// </remarks>
        public class LineJoinStyle {
            private LineJoinStyle() {
            }

            // This private constructor will prevent the instantiation of this class
            /// <summary>
            /// The outer edges of the strokes for the two segments are extended
            /// until they meet at an angle, as in a picture frame.
            /// </summary>
            /// <remarks>
            /// The outer edges of the strokes for the two segments are extended
            /// until they meet at an angle, as in a picture frame. If the segments
            /// meet at too sharp an angle, a bevel join is used instead.
            /// </remarks>
            public const int MITER = 0;

            /// <summary>
            /// An arc of a circle with a diameter equal to the line width is drawn
            /// around the point where the two segments meet, connecting the outer
            /// edges of the strokes for the two segments.
            /// </summary>
            /// <remarks>
            /// An arc of a circle with a diameter equal to the line width is drawn
            /// around the point where the two segments meet, connecting the outer
            /// edges of the strokes for the two segments. This pieslice-shaped
            /// figure is filled in, producing a rounded corner.
            /// </remarks>
            public const int ROUND = 1;

            /// <summary>
            /// The two segments are finished with butt caps (@see LineCapStyle#BUTT)
            /// and the resulting notch beyond the ends of the segments is filled
            /// with a triangle.
            /// </summary>
            public const int BEVEL = 2;
        }

        /// <summary>Rule for determining which points lie inside a path.</summary>
        public class FillingRule {
            private FillingRule() {
            }

            // This private constructor will prevent the instantiation of this class
            /// <summary>The nonzero winding number rule.</summary>
            public const int NONZERO_WINDING = 1;

            /// <summary>The even-odd winding number rule.</summary>
            public const int EVEN_ODD = 2;
        }
    }
}
