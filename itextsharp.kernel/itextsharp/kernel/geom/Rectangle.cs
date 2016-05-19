/*
$Id$

This file is part of the iText (R) project.
Copyright (c) 1998-2016 iText Group NV
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

namespace iTextSharp.Kernel.Geom
{
	public class Rectangle
	{
		private static float EPS = 1e-4f;

		protected internal float x;

		protected internal float y;

		protected internal float width;

		protected internal float height;

		public Rectangle(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public Rectangle(float width, float height)
			: this(0, 0, width, height)
		{
		}

		public Rectangle(iTextSharp.Kernel.Geom.Rectangle rect)
			: this(rect.GetX(), rect.GetY(), rect.GetWidth(), rect.GetHeight())
		{
		}

		/// <summary>Calculates the common rectangle which includes all the input rectangles.
		/// 	</summary>
		/// <param name="rectangles">list of input rectangles.</param>
		/// <returns>common rectangle.</returns>
		public static iTextSharp.Kernel.Geom.Rectangle GetCommonRectangle(params iTextSharp.Kernel.Geom.Rectangle
			[] rectangles)
		{
			float? ury = -float.MaxValue;
			float? llx = float.MaxValue;
			float? lly = float.MaxValue;
			float? urx = -float.MaxValue;
			foreach (iTextSharp.Kernel.Geom.Rectangle rectangle in rectangles)
			{
				if (rectangle == null)
				{
					continue;
				}
				iTextSharp.Kernel.Geom.Rectangle rec = rectangle.Clone();
				if (rec.GetY() < lly)
				{
					lly = rec.GetY();
				}
				if (rec.GetX() < llx)
				{
					llx = rec.GetX();
				}
				if (rec.GetY() + rec.GetHeight() > ury)
				{
					ury = rec.GetY() + rec.GetHeight();
				}
				if (rec.GetX() + rec.GetWidth() > urx)
				{
					urx = rec.GetX() + rec.GetWidth();
				}
			}
			return new iTextSharp.Kernel.Geom.Rectangle(llx, lly, urx - llx, ury - lly);
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle SetBbox(float llx, float lly, float
			 urx, float ury)
		{
			// If llx is greater than urx, swap them (normalize)
			if (llx > urx)
			{
				float temp = llx;
				llx = urx;
				urx = temp;
			}
			// If lly is greater than ury, swap them (normalize)
			if (lly > ury)
			{
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

		public virtual float GetX()
		{
			return x;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle SetX(float x)
		{
			this.x = x;
			return this;
		}

		public virtual float GetY()
		{
			return y;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle SetY(float y)
		{
			this.y = y;
			return this;
		}

		public virtual float GetWidth()
		{
			return width;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle SetWidth(float width)
		{
			this.width = width;
			return this;
		}

		public virtual float GetHeight()
		{
			return height;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle SetHeight(float height)
		{
			this.height = height;
			return this;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle IncreaseHeight(float extra)
		{
			this.height += extra;
			return this;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle DecreaseHeight(float extra)
		{
			this.height -= extra;
			return this;
		}

		/// <summary>
		/// Gets llx, the same:
		/// <c>getX()</c>
		/// .
		/// </summary>
		public virtual float GetLeft()
		{
			return x;
		}

		/// <summary>
		/// Gets urx, the same to
		/// <c>getX() + getWidth()</c>
		/// .
		/// </summary>
		public virtual float GetRight()
		{
			return x + width;
		}

		/// <summary>
		/// Gets ury, the same to
		/// <c>getY() + getHeight()</c>
		/// .
		/// </summary>
		public virtual float GetTop()
		{
			return y + height;
		}

		/// <summary>
		/// Gets lly, the same to
		/// <c>getY()</c>
		/// .
		/// </summary>
		public virtual float GetBottom()
		{
			return y;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle MoveDown(float move)
		{
			y -= move;
			return this;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle MoveUp(float move)
		{
			y += move;
			return this;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle MoveRight(float move)
		{
			x += move;
			return this;
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle MoveLeft(float move)
		{
			x -= move;
			return this;
		}

		public virtual T ApplyMargins<T>(float topIndent, float rightIndent, float bottomIndent
			, float leftIndent, bool reverse)
			where T : iTextSharp.Kernel.Geom.Rectangle
		{
			x += leftIndent * (reverse ? -1 : 1);
			width -= (leftIndent + rightIndent) * (reverse ? -1 : 1);
			y += bottomIndent * (reverse ? -1 : 1);
			height -= (topIndent + bottomIndent) * (reverse ? -1 : 1);
			return (T)this;
		}

		public virtual bool IntersectsLine(float x1, float y1, float x2, float y2)
		{
			double rx1 = GetX();
			double ry1 = GetY();
			double rx2 = rx1 + GetWidth();
			double ry2 = ry1 + GetHeight();
			return (rx1 <= x1 && x1 <= rx2 && ry1 <= y1 && y1 <= ry2) || (rx1 <= x2 && x2 <= 
				rx2 && ry1 <= y2 && y2 <= ry2) || LinesIntersect(rx1, ry1, rx2, ry2, x1, y1, x2, 
				y2) || LinesIntersect(rx2, ry1, rx1, ry2, x1, y1, x2, y2);
		}

		public override String ToString()
		{
			return "Rectangle: " + GetWidth() + 'x' + GetHeight();
		}

		public virtual iTextSharp.Kernel.Geom.Rectangle Clone()
		{
			return new iTextSharp.Kernel.Geom.Rectangle(x, y, width, height);
		}

		public virtual bool EqualsWithEpsilon(iTextSharp.Kernel.Geom.Rectangle that)
		{
			return EqualsWithEpsilon(that, EPS);
		}

		public virtual bool EqualsWithEpsilon(iTextSharp.Kernel.Geom.Rectangle that, float
			 eps)
		{
			float dx = Math.Abs(x - that.x);
			float dy = Math.Abs(y - that.y);
			float dw = Math.Abs(width - that.width);
			float dh = Math.Abs(height - that.height);
			return dx < eps && dy < eps && dw < eps && dh < eps;
		}

		private static bool LinesIntersect(double x1, double y1, double x2, double y2, double
			 x3, double y3, double x4, double y4)
		{
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
			if (AvB == 0.0 && AvC == 0.0)
			{
				if (x2 != 0.0)
				{
					return (x4 * x3 <= 0.0) || ((x3 * x2 >= 0.0) && (x2 > 0.0 ? x3 <= x2 || x4 <= x2 : 
						x3 >= x2 || x4 >= x2));
				}
				if (y2 != 0.0)
				{
					return (y4 * y3 <= 0.0) || ((y3 * y2 >= 0.0) && (y2 > 0.0 ? y3 <= y2 || y4 <= y2 : 
						y3 >= y2 || y4 >= y2));
				}
				return false;
			}
			double BvC = x3 * y4 - x4 * y3;
			return (AvB * AvC <= 0.0) && (BvC * (AvB + BvC - AvC) <= 0.0);
		}
	}
}
