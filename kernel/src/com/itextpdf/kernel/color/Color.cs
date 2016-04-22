/*
$Id: 205c2ea35ca678e05e75594f394b9cfd6ccf01b5 $

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
using com.itextpdf.kernel;
using com.itextpdf.kernel.pdf;
using com.itextpdf.kernel.pdf.colorspace;

namespace com.itextpdf.kernel.color
{
	public class Color
	{
		public static readonly com.itextpdf.kernel.color.Color BLACK = new DeviceRgb(0, 0
			, 0);

		public static readonly com.itextpdf.kernel.color.Color BLUE = new DeviceRgb(0, 0, 
			255);

		public static readonly com.itextpdf.kernel.color.Color CYAN = new DeviceRgb(0, 255
			, 255);

		public static readonly com.itextpdf.kernel.color.Color DARK_GRAY = new DeviceRgb(
			64, 64, 64);

		public static readonly com.itextpdf.kernel.color.Color GRAY = new DeviceRgb(128, 
			128, 128);

		public static readonly com.itextpdf.kernel.color.Color GREEN = new DeviceRgb(0, 255
			, 0);

		public static readonly com.itextpdf.kernel.color.Color LIGHT_GRAY = new DeviceRgb
			(192, 192, 192);

		public static readonly com.itextpdf.kernel.color.Color MAGENTA = new DeviceRgb(255
			, 0, 255);

		public static readonly com.itextpdf.kernel.color.Color ORANGE = new DeviceRgb(255
			, 200, 0);

		public static readonly com.itextpdf.kernel.color.Color PINK = new DeviceRgb(255, 
			175, 175);

		public static readonly com.itextpdf.kernel.color.Color RED = new DeviceRgb(255, 0
			, 0);

		public static readonly com.itextpdf.kernel.color.Color WHITE = new DeviceRgb(255, 
			255, 255);

		public static readonly com.itextpdf.kernel.color.Color YELLOW = new DeviceRgb(255
			, 255, 0);

		protected internal PdfColorSpace colorSpace;

		protected internal float[] colorValue;

		public Color(PdfObject pdfObject, float[] colorValue)
			: this(PdfColorSpace.MakeColorSpace(pdfObject), colorValue)
		{
		}

		public Color(PdfObject pdfObject)
			: this(pdfObject, null)
		{
		}

		public Color(PdfColorSpace colorSpace, float[] colorValue)
		{
			this.colorSpace = colorSpace;
			if (colorValue == null)
			{
				this.colorValue = new float[colorSpace.GetNumberOfComponents()];
			}
			else
			{
				this.colorValue = colorValue;
			}
		}

		public static DeviceRgb ConvertCmykToRgb(DeviceCmyk cmykColor)
		{
			float cyanComp = 1 - cmykColor.GetColorValue()[0];
			float magentaComp = 1 - cmykColor.GetColorValue()[1];
			float yellowComp = 1 - cmykColor.GetColorValue()[2];
			float blackComp = 1 - cmykColor.GetColorValue()[3];
			float r = cyanComp * blackComp;
			float g = magentaComp * blackComp;
			float b = yellowComp * blackComp;
			return new DeviceRgb(r, g, b);
		}

		public static DeviceCmyk ConvertRgbToCmyk(DeviceRgb rgbColor)
		{
			float redComp = rgbColor.GetColorValue()[0];
			float greenComp = rgbColor.GetColorValue()[1];
			float blueComp = rgbColor.GetColorValue()[2];
			float k = 1 - Math.Max(Math.Max(redComp, greenComp), blueComp);
			float c = (1 - redComp - k) / (1 - k);
			float m = (1 - greenComp - k) / (1 - k);
			float y = (1 - blueComp - k) / (1 - k);
			return new DeviceCmyk(c, m, y, k);
		}

		public virtual int GetNumberOfComponents()
		{
			return colorValue.Length;
		}

		public virtual PdfColorSpace GetColorSpace()
		{
			return colorSpace;
		}

		public virtual float[] GetColorValue()
		{
			return colorValue;
		}

		public virtual void SetColorValue(float[] value)
		{
			colorValue = value;
			if (colorValue.Length != value.Length)
			{
				throw new PdfException(PdfException.IncorrectNumberOfComponents, this);
			}
		}

		public override bool Equals(Object o)
		{
			if (this == o)
			{
				return true;
			}
			if (o == null || GetClass() != o.GetClass())
			{
				return false;
			}
			com.itextpdf.kernel.color.Color color = (com.itextpdf.kernel.color.Color)o;
			return (colorSpace != null ? colorSpace.Equals(color.colorSpace) : color.colorSpace
				 == null) && com.itextpdf.io.util.JavaUtil.ArraysEquals(colorValue, color.colorValue
				);
		}

		public override int GetHashCode()
		{
			int result = colorSpace != null ? colorSpace.GetHashCode() : 0;
			result = 31 * result + (colorValue != null ? com.itextpdf.io.util.JavaUtil.ArraysHashCode
				(colorValue) : 0);
			return result;
		}
	}
}
