/*

This file is part of the iText (R) project.
Copyright (c) 1998-2017 iText Group NV
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

namespace iText.Kernel.Geom {
    /// <summary>Class that represent rectangle object.</summary>
    public class Rectangle {
        private static float EPS = 1e-4f;

        protected internal float x;

        protected internal float y;

        protected internal float width;

        protected internal float height;

        /// <summary>Creates new instance.</summary>
        /// <param name="x">the x coordinate of lower left point</param>
        /// <param name="y">the y coordinate of lower left point</param>
        /// <param name="width">the width value</param>
        /// <param name="height">the height value</param>
        public Rectangle(float x, float y, float width, float height) {
            this.x = x;
            this.y = y;
            this.width = width;
            this.height = height;
        }

        /// <summary>Creates new instance of rectangle with (0, 0) as the lower left point.</summary>
        /// <param name="width">the width value</param>
        /// <param name="height">the height value</param>
        public Rectangle(float width, float height)
            : this(0, 0, width, height) {
        }

        /// <summary>
        /// Creates the copy of given
        /// <see cref="Rectangle"/>
        /// </summary>
        /// <param name="rect">
        /// the copied
        /// <see cref="Rectangle"/>
        /// </param>
        public Rectangle(iText.Kernel.Geom.Rectangle rect)
            : this(rect.GetX(), rect.GetY(), rect.GetWidth(), rect.GetHeight()) {
        }

        /// <summary>Calculates the common rectangle which includes all the input rectangles.</summary>
        /// <param name="rectangles">list of input rectangles.</param>
        /// <returns>common rectangle.</returns>
        public static iText.Kernel.Geom.Rectangle GetCommonRectangle(params iText.Kernel.Geom.Rectangle[] rectangles
            ) {
            float ury = -float.MaxValue;
            float llx = float.MaxValue;
            float lly = float.MaxValue;
            float urx = -float.MaxValue;
            foreach (iText.Kernel.Geom.Rectangle rectangle in rectangles) {
                if (rectangle == null) {
                    continue;
                }
                iText.Kernel.Geom.Rectangle rec = rectangle.Clone();
                if (rec.GetY() < lly) {
                    lly = rec.GetY();
                }
                if (rec.GetX() < llx) {
                    llx = rec.GetX();
                }
                if (rec.GetY() + rec.GetHeight() > ury) {
                    ury = rec.GetY() + rec.GetHeight();
                }
                if (rec.GetX() + rec.GetWidth() > urx) {
                    urx = rec.GetX() + rec.GetWidth();
                }
            }
            return new iText.Kernel.Geom.Rectangle(llx, lly, urx - llx, ury - lly);
        }

        /// <summary>Get the rectangle representation of the intersection between this rectangle and the passed rectangle
        ///     </summary>
        /// <param name="rect">the rectangle to find overlap with</param>
        /// <returns>the overlap rectangle if the passed rectangles overlaps with this rectangle, null otherwise</returns>
        public virtual iText.Kernel.Geom.Rectangle GetIntersectionRectangle(iText.Kernel.Geom.Rectangle rect) {
            iText.Kernel.Geom.Rectangle result = null;
            //Calculate possible lower-left corner and upper-right corner
            float llx = Math.Max(x, rect.x);
            float lly = Math.Max(y, rect.y);
            float urx = Math.Min(GetRight(), rect.GetRight());
            float ury = Math.Min(GetTop(), rect.GetTop());
            //If width or height is non-negative, there is overlap and we can construct the intersection rectangle
            //TODO(Samuel) check if this can cause overflow and handle appropriatly
            float width = urx - llx;
            float height = ury - lly;
            if (iText.IO.Util.JavaUtil.FloatCompare(width, 0) >= 0 || iText.IO.Util.JavaUtil.FloatCompare(height, 0) >=
                 0) {
                if (iText.IO.Util.JavaUtil.FloatCompare(width, 0) < 0) {
                    width = 0;
                }
                if (iText.IO.Util.JavaUtil.FloatCompare(height, 0) < 0) {
                    height = 0;
                }
                result = new iText.Kernel.Geom.Rectangle(llx, lly, width, height);
            }
            return result;
        }

        /// <summary>
        /// Check if this rectangle envelops the passed rectangle
        /// A rectangle will envelop itself
        /// </summary>
        /// <param name="rect"/>
        /// <returns>true if this rectangle envelops the passed rectangle, false otherwise.</returns>
        public virtual bool Envelops(iText.Kernel.Geom.Rectangle rect) {
            float llx = this.GetX();
            float lly = this.GetY();
            float urx = llx + this.GetWidth();
            float ury = lly + this.GetHeight();
            float rllx = rect.GetX();
            float rlly = rect.GetY();
            float rurx = rllx + rect.GetWidth();
            float rury = rlly + rect.GetHeight();
            return llx <= rllx && lly <= rlly && rurx <= urx && rury <= ury;
        }

        /// <summary>Check if this rectangle and the passed rectangle overlap</summary>
        /// <param name="rect"/>
        /// <returns>true if there is overlap of some kind</returns>
        public virtual bool Overlaps(iText.Kernel.Geom.Rectangle rect) {
            // Two rectangles do not overlap if any of the following holds
            // 1. the lower left corner of the second rectangle is to the right of the upper-right corner of the first.
            // 2. the lower left corner of the second rectangle is above the upper right corner of the first.
            // 3. the upper right corner of the second rectangle is to the left of the lower-left corner of the first.
            // 4. the upper right corner of the second rectangle is below the lower left corner of the first.
            float llx = this.GetX();
            float lly = this.GetY();
            float urx = llx + this.GetWidth();
            float ury = lly + this.GetHeight();
            float rllx = rect.GetX();
            float rlly = rect.GetY();
            float rurx = rllx + rect.GetWidth();
            float rury = rlly + rect.GetHeight();
            return !(urx < rllx || ury < rlly || llx > rurx || lly > rury);
        }

        /// <summary>Sets the rectangle by the coordinates, specifying its lower left and upper right points.</summary>
        /// <remarks>
        /// Sets the rectangle by the coordinates, specifying its lower left and upper right points. May be used in chain.
        /// <br/>
        /// <br/>
        /// Note: this method will normalize coordinates, so the rectangle will have non negative width and height,
        /// and its x and y coordinates specified lower left point.
        /// </remarks>
        /// <param name="llx">the X coordinate of lower left point</param>
        /// <param name="lly">the Y coordinate of lower left point</param>
        /// <param name="urx">the X coordinate of upper right point</param>
        /// <param name="ury">the Y coordinate of upper right point</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle SetBbox(float llx, float lly, float urx, float ury) {
            // If llx is greater than urx, swap them (normalize)
            if (llx > urx) {
                float temp = llx;
                llx = urx;
                urx = temp;
            }
            // If lly is greater than ury, swap them (normalize)
            if (lly > ury) {
                float temp = lly;
                lly = ury;
                ury = temp;
            }
            x = llx;
            y = lly;
            width = urx - llx;
            height = ury - lly;
            return this;
        }

        /// <summary>Gets the X coordinate of lower left point.</summary>
        /// <returns>the X coordinate of lower left point.</returns>
        public virtual float GetX() {
            return x;
        }

        /// <summary>Sets the X coordinate of lower left point.</summary>
        /// <remarks>Sets the X coordinate of lower left point. May be used in chain.</remarks>
        /// <param name="x">the X coordinate of lower left point to be set.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle SetX(float x) {
            this.x = x;
            return this;
        }

        /// <summary>Gets the Y coordinate of lower left point.</summary>
        /// <returns>the Y coordinate of lower left point.</returns>
        public virtual float GetY() {
            return y;
        }

        /// <summary>Sets the Y coordinate of lower left point.</summary>
        /// <remarks>Sets the Y coordinate of lower left point. May be used in chain.</remarks>
        /// <param name="y">the Y coordinate of lower left point to be set.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle SetY(float y) {
            this.y = y;
            return this;
        }

        /// <summary>Gets the width of rectangle.</summary>
        /// <returns>the width of rectangle.</returns>
        public virtual float GetWidth() {
            return width;
        }

        /// <summary>Sets the width of rectangle.</summary>
        /// <remarks>Sets the width of rectangle. May be used in chain.</remarks>
        /// <param name="width">the the width of rectangle to be set.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle SetWidth(float width) {
            this.width = width;
            return this;
        }

        /// <summary>Gets the height of rectangle.</summary>
        /// <returns>the height of rectangle.</returns>
        public virtual float GetHeight() {
            return height;
        }

        /// <summary>Sets the height of rectangle.</summary>
        /// <remarks>Sets the height of rectangle. May be used in chain.</remarks>
        /// <param name="height">the the width of rectangle to be set.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle SetHeight(float height) {
            this.height = height;
            return this;
        }

        /// <summary>Increases the height of rectangle by the given value.</summary>
        /// <remarks>Increases the height of rectangle by the given value. May be used in chain.</remarks>
        /// <param name="extra">the value of the extra height to be added.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle IncreaseHeight(float extra) {
            this.height += extra;
            return this;
        }

        /// <summary>Decreases the height of rectangle by the given value.</summary>
        /// <remarks>Decreases the height of rectangle by the given value. May be used in chain.</remarks>
        /// <param name="extra">the value of the extra height to be subtracted.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle DecreaseHeight(float extra) {
            this.height -= extra;
            return this;
        }

        /// <summary>Gets the X coordinate of the left edge of the rectangle.</summary>
        /// <remarks>
        /// Gets the X coordinate of the left edge of the rectangle. Same as:
        /// <c>getX()</c>
        /// .
        /// </remarks>
        /// <returns>the X coordinate of the left edge of the rectangle.</returns>
        public virtual float GetLeft() {
            return x;
        }

        /// <summary>Gets the X coordinate of the right edge of the rectangle.</summary>
        /// <remarks>
        /// Gets the X coordinate of the right edge of the rectangle. Same as:
        /// <c>getX() + getWidth()</c>
        /// .
        /// </remarks>
        /// <returns>the X coordinate of the right edge of the rectangle.</returns>
        public virtual float GetRight() {
            return x + width;
        }

        /// <summary>Gets the Y coordinate of the upper edge of the rectangle.</summary>
        /// <remarks>
        /// Gets the Y coordinate of the upper edge of the rectangle. Same as:
        /// <c>getY() + getHeight()</c>
        /// .
        /// </remarks>
        /// <returns>the Y coordinate of the upper edge of the rectangle.</returns>
        public virtual float GetTop() {
            return y + height;
        }

        /// <summary>Gets the Y coordinate of the lower edge of the rectangle.</summary>
        /// <remarks>
        /// Gets the Y coordinate of the lower edge of the rectangle. Same as:
        /// <c>getY()</c>
        /// .
        /// </remarks>
        /// <returns>the Y coordinate of the lower edge of the rectangle.</returns>
        public virtual float GetBottom() {
            return y;
        }

        /// <summary>Decreases the y coordinate.</summary>
        /// <param name="move">the value on which the position will be changed.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle MoveDown(float move) {
            y -= move;
            return this;
        }

        /// <summary>Increases the y coordinate.</summary>
        /// <param name="move">the value on which the position will be changed.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle MoveUp(float move) {
            y += move;
            return this;
        }

        /// <summary>Increases the x coordinate.</summary>
        /// <param name="move">the value on which the position will be changed.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle MoveRight(float move) {
            x += move;
            return this;
        }

        /// <summary>Decreases the x coordinate.</summary>
        /// <param name="move">the value on which the position will be changed.</param>
        /// <returns>
        /// this
        /// <see cref="Rectangle"/>
        /// instance.
        /// </returns>
        public virtual iText.Kernel.Geom.Rectangle MoveLeft(float move) {
            x -= move;
            return this;
        }

        /// <summary>Change the rectangle according the specified margins.</summary>
        /// <param name="topIndent">the value on which the top y coordinate will change.</param>
        /// <param name="rightIndent">the value on which the right x coordinate will change.</param>
        /// <param name="bottomIndent">the value on which the bottom y coordinate will change.</param>
        /// <param name="leftIndent">the value on which the left x coordinate will change.</param>
        /// <param name="reverse">
        /// if
        /// <see langword="true"/>
        /// the rectangle will expand, otherwise it will shrink
        /// </param>
        /// 
        /// <returns>this instance that is cast to type T.</returns>
        public virtual T ApplyMargins<T>(float topIndent, float rightIndent, float bottomIndent, float leftIndent, 
            bool reverse)
            where T : iText.Kernel.Geom.Rectangle {
            x += leftIndent * (reverse ? -1 : 1);
            width -= (leftIndent + rightIndent) * (reverse ? -1 : 1);
            y += bottomIndent * (reverse ? -1 : 1);
            height -= (topIndent + bottomIndent) * (reverse ? -1 : 1);
            return (T)this;
        }

        /// <summary>Checks if rectangle have common points with line, specified by two points.</summary>
        /// <param name="x1">the x coordinate of first line's point.</param>
        /// <param name="y1">the y coordinate of first line's point.</param>
        /// <param name="x2">the x coordinate of second line's point.</param>
        /// <param name="y2">the y coordinate of second line's point.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if rectangle have common points with line and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool IntersectsLine(float x1, float y1, float x2, float y2) {
            double rx1 = GetX();
            double ry1 = GetY();
            double rx2 = rx1 + GetWidth();
            double ry2 = ry1 + GetHeight();
            return (rx1 <= x1 && x1 <= rx2 && ry1 <= y1 && y1 <= ry2) || (rx1 <= x2 && x2 <= rx2 && ry1 <= y2 && y2 <=
                 ry2) || LinesIntersect(rx1, ry1, rx2, ry2, x1, y1, x2, y2) || LinesIntersect(rx2, ry1, rx1, ry2, x1, 
                y1, x2, y2);
        }

        /// <summary>Gets the string representation of rectangle.</summary>
        /// <returns>the string representation of rectangle.</returns>
        public override String ToString() {
            return "Rectangle: " + GetWidth() + 'x' + GetHeight();
        }

        /// <summary>Compare the rectangle to the passed object</summary>
        /// <param name="o">passed object</param>
        /// <returns>True if o is a rectangle and matches in x,y,width and height, false otherwise</returns>
        public override bool Equals(Object o) {
            if (o == null) {
                return false;
            }
            //Cast to rectangle first
            if (!o.GetType().Equals(typeof(iText.Kernel.Geom.Rectangle))) {
                return false;
            }
            iText.Kernel.Geom.Rectangle rect = (iText.Kernel.Geom.Rectangle)o;
            return (iText.IO.Util.JavaUtil.FloatCompare(this.x, rect.GetX()) == 0) && (iText.IO.Util.JavaUtil.FloatCompare
                (this.y, rect.GetY()) == 0) && (iText.IO.Util.JavaUtil.FloatCompare(this.width, rect.GetWidth()) == 0)
                 && (iText.IO.Util.JavaUtil.FloatCompare(this.height, rect.GetHeight()) == 0);
        }

        /// <summary>Gets the copy of this rectangle.</summary>
        /// <returns>the copied rectangle.</returns>
        public virtual iText.Kernel.Geom.Rectangle Clone() {
            return new iText.Kernel.Geom.Rectangle(x, y, width, height);
        }

        /// <summary>Compares instance of this rectangle with given deviation equals to 0.0001</summary>
        /// <param name="that">
        /// the
        /// <see cref="Rectangle"/>
        /// to compare with.
        /// </param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the difference between corresponding rectangle values is less than deviation and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool EqualsWithEpsilon(iText.Kernel.Geom.Rectangle that) {
            return EqualsWithEpsilon(that, EPS);
        }

        /// <summary>Compares instance of this rectangle with given deviation.</summary>
        /// <param name="that">
        /// the
        /// <see cref="Rectangle"/>
        /// to compare with.
        /// </param>
        /// <param name="eps">the deviation value.</param>
        /// <returns>
        /// 
        /// <see langword="true"/>
        /// if the difference between corresponding rectangle values is less than deviation and
        /// <see langword="false"/>
        /// otherwise.
        /// </returns>
        public virtual bool EqualsWithEpsilon(iText.Kernel.Geom.Rectangle that, float eps) {
            float dx = Math.Abs(x - that.x);
            float dy = Math.Abs(y - that.y);
            float dw = Math.Abs(width - that.width);
            float dh = Math.Abs(height - that.height);
            return dx < eps && dy < eps && dw < eps && dh < eps;
        }

        private static bool LinesIntersect(double x1, double y1, double x2, double y2, double x3, double y3, double
             x4, double y4) {
            /*
            * A = (x2-x1, y2-y1) B = (x3-x1, y3-y1) C = (x4-x1, y4-y1) D = (x4-x3,
            * y4-y3) = C-B E = (x1-x3, y1-y3) = -B F = (x2-x3, y2-y3) = A-B
            *
            * Result is ((AxB) * (AxC) <=0) and ((DxE) * (DxF) <= 0)
            *
            * DxE = (C-B)x(-B) = BxB-CxB = BxC DxF = (C-B)x(A-B) = CxA-CxB-BxA+BxB =
            * AxB+BxC-AxC
            */
            x2 -= x1;
            // A
            y2 -= y1;
            x3 -= x1;
            // B
            y3 -= y1;
            x4 -= x1;
            // C
            y4 -= y1;
            double AvB = x2 * y3 - x3 * y2;
            double AvC = x2 * y4 - x4 * y2;
            // Online
            if (AvB == 0.0 && AvC == 0.0) {
                if (x2 != 0.0) {
                    return (x4 * x3 <= 0.0) || ((x3 * x2 >= 0.0) && (x2 > 0.0 ? x3 <= x2 || x4 <= x2 : x3 >= x2 || x4 >= x2));
                }
                if (y2 != 0.0) {
                    return (y4 * y3 <= 0.0) || ((y3 * y2 >= 0.0) && (y2 > 0.0 ? y3 <= y2 || y4 <= y2 : y3 >= y2 || y4 >= y2));
                }
                return false;
            }
            double BvC = x3 * y4 - x4 * y3;
            return (AvB * AvC <= 0.0) && (BvC * (AvB + BvC - AvC) <= 0.0);
        }
    }
}
