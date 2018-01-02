/*

This file is part of the iText (R) project.
Copyright (c) 1998-2018 iText Group NV
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
using System;
using Common.Logging;
using iText.IO.Util;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;

namespace iText.Layout.Borders {
    /// <summary>Represents a border.</summary>
    public abstract class Border {
        /// <summary>The null Border, i.e.</summary>
        /// <remarks>The null Border, i.e. the presence of such border is equivalent to the absence of the border</remarks>
        public static readonly iText.Layout.Borders.Border NO_BORDER = null;

        /// <summary>The solid border.</summary>
        /// <seealso cref="SolidBorder"/>
        public const int SOLID = 0;

        /// <summary>The dashed border.</summary>
        /// <seealso cref="DashedBorder"/>
        public const int DASHED = 1;

        /// <summary>The dotted border.</summary>
        /// <seealso cref="DottedBorder"/>
        public const int DOTTED = 2;

        /// <summary>The double border.</summary>
        /// <seealso cref="DoubleBorder"/>
        public const int DOUBLE = 3;

        /// <summary>The round-dots border.</summary>
        /// <seealso cref="RoundDotsBorder"/>
        public const int ROUND_DOTS = 4;

        /// <summary>The 3D groove border.</summary>
        /// <seealso cref="GrooveBorder"/>
        public const int _3D_GROOVE = 5;

        /// <summary>The 3D inset border.</summary>
        /// <seealso cref="InsetBorder"/>
        public const int _3D_INSET = 6;

        /// <summary>The 3D outset border.</summary>
        /// <seealso cref="OutsetBorder"/>
        public const int _3D_OUTSET = 7;

        /// <summary>The 3D ridge border.</summary>
        /// <seealso cref="RidgeBorder"/>
        public const int _3D_RIDGE = 8;

        /// <summary>The color of the border.</summary>
        /// <seealso cref="iText.Layout.Properties.TransparentColor"/>
        protected internal TransparentColor transparentColor;

        /// <summary>The width of the border.</summary>
        protected internal float width;

        /// <summary>The type of the border.</summary>
        protected internal int type;

        /// <summary>The hash value for the border.</summary>
        private int hash;

        /// <summary>
        /// Creates a
        /// <see cref="Border">border</see>
        /// with the given width.
        /// The
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// to be set by default is black
        /// </summary>
        /// <param name="width">the width which the border should have</param>
        protected internal Border(float width)
            : this(ColorConstants.BLACK, width) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="Border">border</see>
        /// with given width and
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// .
        /// </summary>
        /// <param name="color">the color which the border should have</param>
        /// <param name="width">the width which the border should have</param>
        protected internal Border(Color color, float width) {
            this.transparentColor = new TransparentColor(color);
            this.width = width;
        }

        /// <summary>
        /// Creates a
        /// <see cref="Border">border</see>
        /// with given width,
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// and opacity.
        /// </summary>
        /// <param name="color">the color which the border should have</param>
        /// <param name="width">the width which the border should have</param>
        /// <param name="opacity">the opacity which border should have; a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent
        ///     </param>
        protected internal Border(Color color, float width, float opacity) {
            this.transparentColor = new TransparentColor(color, opacity);
            this.width = width;
        }

        /// <summary>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// </p>
        /// <p>
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// </p>
        /// <p>
        /// <code>borderWidthBefore</code> and <code>borderWidthAfter</code> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <code>borderWidthBefore</code> specifies width of the right border and
        /// <code>borderWidthAfter</code> - width of the left border. Those width are used to handle areas
        /// of border joins.
        /// </p>
        /// </remarks>
        /// <param name="canvas">PdfCanvas to be written to</param>
        /// <param name="x1">x coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="y1">y coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="x2">x coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="y2">y coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="defaultSide">
        /// the
        /// <see cref="Side"/>
        /// , that we will fallback to, if it cannot be determined by border coordinates
        /// </param>
        /// <param name="borderWidthBefore">defines width of the border that is before the current one</param>
        /// <param name="borderWidthAfter">defines width of the border that is after the current one</param>
        public abstract void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side defaultSide
            , float borderWidthBefore, float borderWidthAfter);

        /// <summary>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// </p>
        /// <p>
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// </p>
        /// <p>
        /// <code>borderWidthBefore</code> and <code>borderWidthAfter</code> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <code>borderWidthBefore</code> specifies width of the right border and
        /// <code>borderWidthAfter</code> - width of the left border. Those width are used to handle areas
        /// of border joins.
        /// </p>
        /// <p>
        /// <code>borderRadius</code> is used to draw rounded borders.
        /// </p>
        /// </remarks>
        /// <param name="canvas">PdfCanvas to be written to</param>
        /// <param name="x1">x coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="y1">y coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="x2">x coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="y2">y coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="borderRadius">defines the radius of the element's corners</param>
        /// <param name="defaultSide">
        /// the
        /// <see cref="Side"/>
        /// , that we will fallback to, if it cannot be determined by border coordinates
        /// </param>
        /// <param name="borderWidthBefore">defines width of the border that is before the current one</param>
        /// <param name="borderWidthAfter">defines width of the border that is after the current one</param>
        public virtual void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float borderRadius, Border.Side
             defaultSide, float borderWidthBefore, float borderWidthAfter) {
            Draw(canvas, x1, y1, x2, y2, borderRadius, borderRadius, borderRadius, borderRadius, defaultSide, borderWidthBefore
                , borderWidthAfter);
        }

        /// <summary>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// <p>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// </p>
        /// <p>
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// </p>
        /// <p>
        /// <code>borderWidthBefore</code> and <code>borderWidthAfter</code> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <code>borderWidthBefore</code> specifies width of the right border and
        /// <code>borderWidthAfter</code> - width of the left border. Those width are used to handle areas
        /// of border joins.
        /// </p>
        /// <p>
        /// <code>horizontalRadius1</code>, <code>verticalRadius1</code>, <code>horizontalRadius2</code>
        /// and <code>verticalRadius2</code> are used to draw rounded borders.
        /// </p>
        /// </remarks>
        /// <param name="canvas">PdfCanvas to be written to</param>
        /// <param name="x1">x coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="y1">y coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="x2">x coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="y2">y coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="horizontalRadius1">defines the horizontal radius of the border's first corner</param>
        /// <param name="verticalRadius1">defines the vertical radius of the border's first corner</param>
        /// <param name="horizontalRadius2">defines the horizontal radius of the border's second corner</param>
        /// <param name="verticalRadius2">defines the vertical radius of the border's second corner</param>
        /// <param name="defaultSide">
        /// the
        /// <see cref="Side"/>
        /// , that we will fallback to, if it cannot be determined by border coordinates
        /// </param>
        /// <param name="borderWidthBefore">defines width of the border that is before the current one</param>
        /// <param name="borderWidthAfter">defines width of the border that is after the current one</param>
        public virtual void Draw(PdfCanvas canvas, float x1, float y1, float x2, float y2, float horizontalRadius1
            , float verticalRadius1, float horizontalRadius2, float verticalRadius2, Border.Side defaultSide, float
             borderWidthBefore, float borderWidthAfter) {
            ILog logger = LogManager.GetLogger(typeof(iText.Layout.Borders.Border));
            logger.Warn(MessageFormatUtil.Format(iText.IO.LogMessageConstant.METHOD_IS_NOT_IMPLEMENTED_BY_DEFAULT_OTHER_METHOD_WILL_BE_USED
                , "Border#draw(PdfCanvas, float, float, float, float, float, float, float, float, Side, float, float", 
                "Border#draw(PdfCanvas, float, float, float, float, Side, float, float)"));
            Draw(canvas, x1, y1, x2, y2, defaultSide, borderWidthBefore, borderWidthAfter);
        }

        /// <summary>Draws the border of a cell.</summary>
        /// <param name="canvas">PdfCanvas to be written to</param>
        /// <param name="x1">x coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="y1">y coordinate of the beginning point of the element side, that should be bordered</param>
        /// <param name="x2">x coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="y2">y coordinate of the ending point of the element side, that should be bordered</param>
        /// <param name="defaultSide">
        /// the
        /// <see cref="Side"/>
        /// , that we will fallback to, if it cannot be determined by border coordinates
        /// </param>
        public abstract void DrawCellBorder(PdfCanvas canvas, float x1, float y1, float x2, float y2, Border.Side 
            defaultSide);

        /// <summary>
        /// Returns the type of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <returns>the type of border.</returns>
        public abstract int GetBorderType();

        /// <summary>
        /// Gets the
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <returns>
        /// the
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// </returns>
        public virtual Color GetColor() {
            return transparentColor.GetColor();
        }

        /// <summary>
        /// Gets the opacity of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <returns>the border opacity; a float between 0 and 1, where 1 stands for fully opaque color and 0 - for fully transparent
        ///     </returns>
        public virtual float GetOpacity() {
            return transparentColor.GetOpacity();
        }

        /// <summary>
        /// Gets the width of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <returns>the width</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>
        /// Sets the
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <param name="color">The color</param>
        public virtual void SetColor(Color color) {
            this.transparentColor = new TransparentColor(color, this.transparentColor.GetOpacity());
        }

        /// <summary>
        /// Sets the width of the
        /// <see cref="Border">border</see>
        /// </summary>
        /// <param name="width">The width</param>
        public virtual void SetWidth(float width) {
            this.width = width;
        }

        /// <summary>Indicates whether the border is equal to the given border.</summary>
        /// <remarks>
        /// Indicates whether the border is equal to the given border.
        /// The border type, width and color are considered during the comparison.
        /// </remarks>
        public override bool Equals(Object anObject) {
            if (this == anObject) {
                return true;
            }
            if (anObject is iText.Layout.Borders.Border) {
                iText.Layout.Borders.Border anotherBorder = (iText.Layout.Borders.Border)anObject;
                if (anotherBorder.GetBorderType() != GetBorderType() || !anotherBorder.GetColor().Equals(GetColor()) || anotherBorder
                    .GetWidth() != GetWidth() || anotherBorder.transparentColor.GetOpacity() != transparentColor.GetOpacity
                    ()) {
                    return false;
                }
            }
            else {
                return false;
            }
            return true;
        }

        /// <summary><inheritDoc/></summary>
        public override int GetHashCode() {
            int h = hash;
            if (h == 0) {
                h = (int)GetWidth() * 31 + GetColor().GetHashCode();
                h = h * 31 + (int)transparentColor.GetOpacity();
                hash = h;
            }
            return h;
        }

        /// <summary>
        /// Returns the
        /// <see cref="Side">side</see>
        /// corresponded to the line between two points.
        /// Notice that we consider the rectangle traversal to be clockwise.
        /// In case side couldn't be detected we will fallback to default side
        /// </summary>
        /// <param name="x1">the abscissa of the left-bottom point</param>
        /// <param name="y1">the ordinate of the left-bottom point</param>
        /// <param name="x2">the abscissa of the right-top point</param>
        /// <param name="y2">the ordinate of the right-top point</param>
        /// <param name="defaultSide">the default side of border</param>
        /// <returns>
        /// the corresponded
        /// <see cref="Side">side</see>
        /// </returns>
        protected internal virtual Border.Side GetBorderSide(float x1, float y1, float x2, float y2, Border.Side defaultSide
            ) {
            bool isLeft = false;
            bool isRight = false;
            if (Math.Abs(y2 - y1) > 0.0005f) {
                isLeft = y2 - y1 > 0;
                isRight = y2 - y1 < 0;
            }
            bool isTop = false;
            bool isBottom = false;
            if (Math.Abs(x2 - x1) > 0.0005f) {
                isTop = x2 - x1 > 0;
                isBottom = x2 - x1 < 0;
            }
            if (isTop) {
                return isLeft ? Border.Side.LEFT : Border.Side.TOP;
            }
            else {
                if (isRight) {
                    return Border.Side.RIGHT;
                }
                else {
                    if (isBottom) {
                        return Border.Side.BOTTOM;
                    }
                    else {
                        if (isLeft) {
                            return Border.Side.LEFT;
                        }
                    }
                }
            }
            return defaultSide;
        }

        /// <summary>Enumerates the different sides of the rectangle.</summary>
        /// <remarks>
        /// Enumerates the different sides of the rectangle.
        /// The rectangle sides are expected to be parallel to corresponding page sides
        /// Otherwise the result is Side.NONE
        /// </remarks>
        public enum Side {
            NONE,
            TOP,
            RIGHT,
            BOTTOM,
            LEFT
        }

        protected internal virtual Point GetIntersectionPoint(Point lineBeg, Point lineEnd, Point clipLineBeg, Point
             clipLineEnd) {
            double A1 = lineBeg.GetY() - lineEnd.GetY();
            double A2 = clipLineBeg.GetY() - clipLineEnd.GetY();
            double B1 = lineEnd.GetX() - lineBeg.GetX();
            double B2 = clipLineEnd.GetX() - clipLineBeg.GetX();
            double C1 = lineBeg.GetX() * lineEnd.GetY() - lineBeg.GetY() * lineEnd.GetX();
            double C2 = clipLineBeg.GetX() * clipLineEnd.GetY() - clipLineBeg.GetY() * clipLineEnd.GetX();
            double M = B1 * A2 - B2 * A1;
            return new Point((B2 * C1 - B1 * C2) / M, (C2 * A1 - C1 * A2) / M);
        }
    }
}
