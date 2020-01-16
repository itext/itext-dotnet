/*

This file is part of the iText (R) project.
Copyright (c) 1998-2020 iText Group NV
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
namespace com.itextpdf.layout.layout
{
	[System.ObsoleteAttribute(@"Will be removed in iText 7.2. Use iText.Kernel.Geom.Rectangle instead.")]
	public class LayoutRect
	{
		protected internal float x;

		protected internal float y;

		protected internal float width;

		protected internal float height;

		public LayoutRect(float x, float y, float width, float height)
		{
			this.x = x;
			this.y = y;
			this.width = width;
			this.height = height;
		}

		public LayoutRect(com.itextpdf.layout.layout.LayoutRect lr)
		{
			this.x = lr.x;
			this.y = lr.y;
			this.width = lr.width;
			this.height = lr.height;
		}

		/// <summary>Calculates the common rectangle which includes all the input rectangles.
		/// 	</summary>
		/// <param name="rectangles">list of input rectangles.</param>
		/// <returns>common rectangle.</returns>
		public static com.itextpdf.layout.layout.LayoutRect GetCommonRectangle(params com.itextpdf.layout.layout.LayoutRect
			[] rectangles)
		{
			float ury = -float.MaxValue;
			float llx = float.MaxValue;
			float lly = float.MaxValue;
			float urx = -float.MaxValue;
			foreach (com.itextpdf.layout.layout.LayoutRect rectangle in rectangles)
			{
				com.itextpdf.layout.layout.LayoutRect rec = new com.itextpdf.layout.layout.LayoutRect
					(rectangle);
				if (rec.GetHeight() == null)
				{
					rec.SetHeight(0f);
				}
				if (rec.GetWidth() == null)
				{
					rec.SetWidth(0f);
				}
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
			return new com.itextpdf.layout.layout.LayoutRect(llx, lly, urx - llx, ury - lly);
		}

		public LayoutRect(float width, float height)
			: this((float)0, (float)0, width, height)
		{
		}

		public virtual float GetX()
		{
			return x;
		}

		public virtual void SetX(float x)
		{
			this.x = x;
		}

		public virtual float GetY()
		{
			return y;
		}

		public virtual void SetY(float y)
		{
			this.y = y;
		}

		public virtual float GetWidth()
		{
			return width;
		}

		public virtual void SetWidth(float width)
		{
			this.width = width;
		}

		public virtual float GetHeight()
		{
			return height;
		}

		public virtual void SetHeight(float height)
		{
			this.height = height;
		}
	}
}
