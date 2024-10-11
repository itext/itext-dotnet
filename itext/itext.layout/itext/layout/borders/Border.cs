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
using Microsoft.Extensions.Logging;
using iText.Commons;
using iText.Commons.Utils;
using iText.Kernel.Colors;
using iText.Kernel.Geom;
using iText.Kernel.Pdf.Canvas;
using iText.Layout.Properties;

namespace iText.Layout.Borders {
    /// <summary>Represents a border.</summary>
    public abstract class Border {
        /// <summary>The null Border, i.e. the presence of such border is equivalent to the absence of the border</summary>
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

        /// <summary>The fixed dashed border.</summary>
        /// <seealso cref="FixedDashedBorder"/>
        public const int DASHED_FIXED = 9;

        private const int ARC_RIGHT_DEGREE = 0;

        private const int ARC_TOP_DEGREE = 90;

        private const int ARC_LEFT_DEGREE = 180;

        private const int ARC_BOTTOM_DEGREE = 270;

        private const int ARC_QUARTER_CLOCKWISE_EXTENT = -90;

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
        /// </summary>
        /// <remarks>
        /// Creates a
        /// <see cref="Border">border</see>
        /// with the given width.
        /// The
        /// <see cref="iText.Kernel.Colors.Color">color</see>
        /// to be set by default is black
        /// </remarks>
        /// <param name="width">the width which the border should have</param>
        protected internal Border(float width)
            : this(ColorConstants.BLACK, width) {
        }

        /// <summary>
        /// Creates a
        /// <see cref="Border">border</see>
        /// with given width and
        /// <see cref="iText.Kernel.Colors.Color">color</see>.
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
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// <para />
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// <para />
        /// <c>borderWidthBefore</c> and <c>borderWidthAfter</c> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <c>borderWidthBefore</c> specifies width of the right border and
        /// <c>borderWidthAfter</c> - width of the left border. Those width are used to handle areas
        /// of border joins.
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

        /// <summary>Draw borders around the target rectangle.</summary>
        /// <param name="canvas">PdfCanvas to be written to</param>
        /// <param name="rectangle">border positions rectangle</param>
        public virtual void Draw(PdfCanvas canvas, Rectangle rectangle) {
            float left = rectangle.GetX();
            float bottom = rectangle.GetY();
            float right = rectangle.GetX() + rectangle.GetWidth();
            float top = rectangle.GetY() + rectangle.GetHeight();
            Draw(canvas, left, top, right, top, Border.Side.TOP, width, width);
            Draw(canvas, right, top, right, bottom, Border.Side.RIGHT, width, width);
            Draw(canvas, right, bottom, left, bottom, Border.Side.BOTTOM, width, width);
            Draw(canvas, left, bottom, left, top, Border.Side.LEFT, width, width);
        }

        /// <summary>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// <para />
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// <para />
        /// <c>borderWidthBefore</c> and <c>borderWidthAfter</c> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <c>borderWidthBefore</c> specifies width of the right border and
        /// <c>borderWidthAfter</c> - width of the left border. Those width are used to handle areas
        /// of border joins.
        /// <para />
        /// <c>borderRadius</c> is used to draw rounded borders.
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
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction.
        /// </summary>
        /// <remarks>
        /// All borders are supposed to be drawn in such way, that inner content of the element is on the right from the
        /// drawing direction. Borders are drawn in this order: top, right, bottom, left.
        /// <para />
        /// Given points specify the line which lies on the border of the content area,
        /// therefore the border itself should be drawn to the left from the drawing direction.
        /// <para />
        /// <c>borderWidthBefore</c> and <c>borderWidthAfter</c> parameters are used to
        /// define the widths of the borders that are before and after the current border, e.g. for
        /// the bottom border, <c>borderWidthBefore</c> specifies width of the right border and
        /// <c>borderWidthAfter</c> - width of the left border. Those width are used to handle areas
        /// of border joins.
        /// <para />
        /// <c>horizontalRadius1</c>, <c>verticalRadius1</c>, <c>horizontalRadius2</c>
        /// and <c>verticalRadius2</c> are used to draw rounded borders.
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
            ILogger logger = ITextLogManager.GetLogger(typeof(iText.Layout.Borders.Border));
            logger.LogWarning(MessageFormatUtil.Format(iText.IO.Logs.IoLogMessageConstant.METHOD_IS_NOT_IMPLEMENTED_BY_DEFAULT_OTHER_METHOD_WILL_BE_USED
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
        /// </summary>
        /// <remarks>
        /// Returns the
        /// <see cref="Side">side</see>
        /// corresponded to the line between two points.
        /// Notice that we consider the rectangle traversal to be clockwise.
        /// In case side couldn't be detected we will fallback to default side
        /// </remarks>
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

        /// <summary>
        /// Gets a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// in which two lines intersect.
        /// </summary>
        /// <param name="lineBeg">
        /// a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which defines some point on the first line
        /// </param>
        /// <param name="lineEnd">
        /// a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which defines another point on the first line
        /// </param>
        /// <param name="clipLineBeg">
        /// a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which defines some point on the second line
        /// </param>
        /// <param name="clipLineEnd">
        /// a
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// which defines another point on the second line
        /// </param>
        /// <returns>
        /// the intersection
        /// <see cref="iText.Kernel.Geom.Point"/>
        /// </returns>
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

        /// <summary>Adjusts the size of the gap between dots</summary>
        /// <param name="distance">
        /// the
        /// <see cref="Border">border</see>
        /// length
        /// </param>
        /// <param name="initialGap">the initial size of the gap</param>
        /// <returns>the adjusted size of the gap</returns>
        protected internal virtual float GetDotsGap(double distance, float initialGap) {
            double gapsNum = Math.Ceiling(distance / initialGap);
            if (gapsNum == 0) {
                return initialGap;
            }
            return (float)(distance / gapsNum);
        }

        /// <summary>Perform drawing operations to draw discontinuous borders.</summary>
        /// <remarks>
        /// Perform drawing operations to draw discontinuous borders. Used by
        /// <see cref="DashedBorder"/>
        /// ,
        /// <see cref="DottedBorder"/>
        /// and
        /// <see cref="RoundDotsBorder"/>.
        /// </remarks>
        /// <param name="canvas">canvas to draw on</param>
        /// <param name="boundingRectangle">rectangle representing the bounding box of the drawing operations</param>
        /// <param name="horizontalRadii">the horizontal radius of the border's two corners</param>
        /// <param name="verticalRadii">the vertical radius of the border's two corners</param>
        /// <param name="defaultSide">
        /// the
        /// <see cref="Side"/>
        /// , that we will fallback to, if it cannot be determined by border coordinates
        /// </param>
        /// <param name="borderWidthBefore">defines width of the border that is before the current one</param>
        /// <param name="borderWidthAfter">defines width of the border that is after the current one</param>
        protected internal virtual void DrawDiscontinuousBorders(PdfCanvas canvas, Rectangle boundingRectangle, float
            [] horizontalRadii, float[] verticalRadii, Border.Side defaultSide, float borderWidthBefore, float borderWidthAfter
            ) {
            double x1 = boundingRectangle.GetX();
            double y1 = boundingRectangle.GetY();
            double x2 = boundingRectangle.GetRight();
            double y2 = boundingRectangle.GetTop();
            double horizontalRadius1 = horizontalRadii[0];
            double horizontalRadius2 = horizontalRadii[1];
            double verticalRadius1 = verticalRadii[0];
            double verticalRadius2 = verticalRadii[1];
            // Points (x0, y0) and (x3, y3) are used to produce Bezier curve
            double x0 = boundingRectangle.GetX();
            double y0 = boundingRectangle.GetY();
            double x3 = boundingRectangle.GetRight();
            double y3 = boundingRectangle.GetTop();
            double innerRadiusBefore;
            double innerRadiusFirst;
            double innerRadiusSecond;
            double innerRadiusAfter;
            double widthHalf = width / 2.0;
            Point clipPoint1;
            Point clipPoint2;
            Point clipPoint;
            Border.Side borderSide = GetBorderSide((float)x1, (float)y1, (float)x2, (float)y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    x0 -= borderWidthBefore / 2;
                    y0 -= innerRadiusFirst;
                    x3 += borderWidthAfter / 2;
                    y3 -= innerRadiusSecond;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 - borderWidthBefore, y1 + width), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0 + 10, y0));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 + borderWidthAfter, y2 + width), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3 - 10, y3));
                    if (clipPoint1.GetX() > clipPoint2.GetX()) {
                        clipPoint = GetIntersectionPoint(new Point(x1 - borderWidthBefore, y1 + width), clipPoint1, clipPoint2, new 
                            Point(x2 + borderWidthAfter, y2 + width));
                        canvas.MoveTo(x1 - borderWidthBefore, y1 + width).LineTo(clipPoint.GetX(), clipPoint.GetY()).LineTo(x2 + borderWidthAfter
                            , y2 + width).LineTo(x1 - borderWidthBefore, y1 + width);
                    }
                    else {
                        canvas.MoveTo(x1 - borderWidthBefore, y1 + width).LineTo(clipPoint1.GetX(), clipPoint1.GetY()).LineTo(clipPoint2
                            .GetX(), clipPoint2.GetY()).LineTo(x2 + borderWidthAfter, y2 + width).LineTo(x1 - borderWidthBefore, y1
                             + width);
                    }
                    canvas.Clip().EndPath();
                    x1 += innerRadiusBefore;
                    y1 += widthHalf;
                    x2 -= innerRadiusAfter;
                    y2 += widthHalf;
                    canvas.Arc(x0, y0 - innerRadiusFirst, x1 + innerRadiusBefore, y1, ARC_LEFT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        ).ArcContinuous(x2 - innerRadiusAfter, y2, x3, y3 - innerRadiusSecond, ARC_TOP_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        );
                    break;
                }

                case Border.Side.RIGHT: {
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    x0 -= innerRadiusFirst;
                    y0 += borderWidthBefore / 2;
                    x3 -= innerRadiusSecond;
                    y3 -= borderWidthAfter / 2;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 + width, y1 + borderWidthBefore), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0, y0 - 10));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 + width, y2 - borderWidthAfter), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3, y3 - 10));
                    if (clipPoint1.GetY() < clipPoint2.GetY()) {
                        clipPoint = GetIntersectionPoint(new Point(x1 + width, y1 + borderWidthBefore), clipPoint1, clipPoint2, new 
                            Point(x2 + width, y2 - borderWidthAfter));
                        canvas.MoveTo(x1 + width, y1 + borderWidthBefore).LineTo(clipPoint.GetX(), clipPoint.GetY()).LineTo(x2 + width
                            , y2 - borderWidthAfter).LineTo(x1 + width, y1 + borderWidthBefore).Clip().EndPath();
                    }
                    else {
                        canvas.MoveTo(x1 + width, y1 + borderWidthBefore).LineTo(clipPoint1.GetX(), clipPoint1.GetY()).LineTo(clipPoint2
                            .GetX(), clipPoint2.GetY()).LineTo(x2 + width, y2 - borderWidthAfter).LineTo(x1 + width, y1 + borderWidthBefore
                            ).Clip().EndPath();
                    }
                    canvas.Clip().EndPath();
                    x1 += widthHalf;
                    y1 -= innerRadiusBefore;
                    x2 += widthHalf;
                    y2 += innerRadiusAfter;
                    canvas.Arc(x0 - innerRadiusFirst, y0, x1, y1 - innerRadiusBefore, ARC_TOP_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        ).ArcContinuous(x2, y2 + innerRadiusAfter, x3 - innerRadiusSecond, y3, ARC_RIGHT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        );
                    break;
                }

                case Border.Side.BOTTOM: {
                    innerRadiusBefore = Math.Max(0, horizontalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, verticalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, verticalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, horizontalRadius2 - borderWidthAfter);
                    x0 += borderWidthBefore / 2;
                    y0 += innerRadiusFirst;
                    x3 -= borderWidthAfter / 2;
                    y3 += innerRadiusSecond;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 + borderWidthBefore, y1 - width), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0 - 10, y0));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 - borderWidthAfter, y2 - width), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3 + 10, y3));
                    if (clipPoint1.GetX() < clipPoint2.GetX()) {
                        clipPoint = GetIntersectionPoint(new Point(x1 + borderWidthBefore, y1 - width), clipPoint1, clipPoint2, new 
                            Point(x2 - borderWidthAfter, y2 - width));
                        canvas.MoveTo(x1 + borderWidthBefore, y1 - width).LineTo(clipPoint.GetX(), clipPoint.GetY()).LineTo(x2 - borderWidthAfter
                            , y2 - width).LineTo(x1 + borderWidthBefore, y1 - width);
                    }
                    else {
                        canvas.MoveTo(x1 + borderWidthBefore, y1 - width).LineTo(clipPoint1.GetX(), clipPoint1.GetY()).LineTo(clipPoint2
                            .GetX(), clipPoint2.GetY()).LineTo(x2 - borderWidthAfter, y2 - width).LineTo(x1 + borderWidthBefore, y1
                             - width);
                    }
                    canvas.Clip().EndPath();
                    x1 -= innerRadiusBefore;
                    y1 -= widthHalf;
                    x2 += innerRadiusAfter;
                    y2 -= widthHalf;
                    canvas.Arc(x0, y0 + innerRadiusFirst, x1 - innerRadiusBefore, y1, ARC_RIGHT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        ).ArcContinuous(x2 + innerRadiusAfter, y2, x3, y3 + innerRadiusSecond, ARC_BOTTOM_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        );
                    break;
                }

                case Border.Side.LEFT: {
                    innerRadiusBefore = Math.Max(0, verticalRadius1 - borderWidthBefore);
                    innerRadiusFirst = Math.Max(0, horizontalRadius1 - width);
                    innerRadiusSecond = Math.Max(0, horizontalRadius2 - width);
                    innerRadiusAfter = Math.Max(0, verticalRadius2 - borderWidthAfter);
                    x0 += innerRadiusFirst;
                    y0 -= borderWidthBefore / 2;
                    x3 += innerRadiusSecond;
                    y3 += borderWidthAfter / 2;
                    clipPoint1 = GetIntersectionPoint(new Point(x1 - width, y1 - borderWidthBefore), new Point(x1, y1), new Point
                        (x0, y0), new Point(x0, y0 + 10));
                    clipPoint2 = GetIntersectionPoint(new Point(x2 - width, y2 + borderWidthAfter), new Point(x2, y2), new Point
                        (x3, y3), new Point(x3, y3 + 10));
                    if (clipPoint1.GetY() > clipPoint2.GetY()) {
                        clipPoint = GetIntersectionPoint(new Point(x1 - width, y1 - borderWidthBefore), clipPoint1, clipPoint2, new 
                            Point(x2 - width, y2 + borderWidthAfter));
                        canvas.MoveTo(x1 - width, y1 - borderWidthBefore).LineTo(clipPoint.GetX(), clipPoint.GetY()).LineTo(x2 - width
                            , y2 + borderWidthAfter).LineTo(x1 - width, y1 - borderWidthBefore);
                    }
                    else {
                        canvas.MoveTo(x1 - width, y1 - borderWidthBefore).LineTo(clipPoint1.GetX(), clipPoint1.GetY()).LineTo(clipPoint2
                            .GetX(), clipPoint2.GetY()).LineTo(x2 - width, y2 + borderWidthAfter).LineTo(x1 - width, y1 - borderWidthBefore
                            );
                    }
                    canvas.Clip().EndPath();
                    x1 -= widthHalf;
                    y1 += innerRadiusBefore;
                    x2 -= widthHalf;
                    y2 -= innerRadiusAfter;
                    canvas.Arc(x0 + innerRadiusFirst, y0, x1, y1 + innerRadiusBefore, ARC_BOTTOM_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        ).ArcContinuous(x2, y2 - innerRadiusAfter, x3 + innerRadiusSecond, y3, ARC_LEFT_DEGREE, ARC_QUARTER_CLOCKWISE_EXTENT
                        );
                    break;
                }

                default: {
                    break;
                }
            }
            canvas.Stroke().RestoreState();
        }

        /// <summary>Calculate adjusted starting points for discontinuous borders, given two opposing points (A and B) that define the bounding rectangle
        ///     </summary>
        /// <param name="x1">x-coordinate of point A</param>
        /// <param name="y1">y-ordinate of point A</param>
        /// <param name="x2">x-coordinate of point B</param>
        /// <param name="y2">y-ordinate of point B</param>
        /// <param name="defaultSide">default side of the border used to determine the side given by points A and B</param>
        /// <returns>float[] containing the adjusted starting points in the form {x1,y1,x2,y2}</returns>
        protected internal virtual float[] GetStartingPointsForBorderSide(float x1, float y1, float x2, float y2, 
            Border.Side defaultSide) {
            float widthHalf = width / 2;
            Border.Side borderSide = GetBorderSide(x1, y1, x2, y2, defaultSide);
            switch (borderSide) {
                case Border.Side.TOP: {
                    y1 += widthHalf;
                    y2 += widthHalf;
                    break;
                }

                case Border.Side.RIGHT: {
                    x1 += widthHalf;
                    x2 += widthHalf;
                    break;
                }

                case Border.Side.BOTTOM: {
                    y1 -= widthHalf;
                    y2 -= widthHalf;
                    break;
                }

                case Border.Side.LEFT: {
                    x1 -= widthHalf;
                    x2 -= widthHalf;
                    break;
                }

                default: {
                    break;
                }
            }
            return new float[] { x1, y1, x2, y2 };
        }
    }
}
