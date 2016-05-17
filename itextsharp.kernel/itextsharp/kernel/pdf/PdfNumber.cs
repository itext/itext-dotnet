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
using iTextSharp.IO.Source;

namespace iTextSharp.Kernel.Pdf
{
	public class PdfNumber : PdfPrimitiveObject
	{
		private double value;

		private bool isDouble;

		public PdfNumber(double value)
			: base()
		{
			SetValue(value);
		}

		public PdfNumber(int value)
			: base()
		{
			SetValue(value);
		}

		public PdfNumber(byte[] content)
			: base(content)
		{
			this.isDouble = true;
			this.value = Double.NaN;
		}

		private PdfNumber()
			: base()
		{
		}

		public override byte GetType()
		{
			return NUMBER;
		}

		public virtual double GetValue()
		{
			if (double.IsNaN(value))
			{
				GenerateValue();
			}
			return value;
		}

		public virtual double DoubleValue()
		{
			return GetValue();
		}

		public virtual float FloatValue()
		{
			return (float)GetValue();
		}

		public virtual long LongValue()
		{
			return (long)GetValue();
		}

		public virtual int IntValue()
		{
			return (int)GetValue();
		}

		public virtual void SetValue(int value)
		{
			this.value = value;
			this.isDouble = false;
			this.content = null;
		}

		public virtual void SetValue(double value)
		{
			this.value = value;
			this.isDouble = true;
			this.content = null;
		}

		public virtual void Increment()
		{
			SetValue(++value);
		}

		public virtual void Decrement()
		{
			SetValue(--value);
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document)
		{
			return (iTextSharp.Kernel.Pdf.PdfNumber)base.MakeIndirect(document);
		}

		/// <summary>Marks object to be saved as indirect.</summary>
		/// <param name="document">a document the indirect reference will belong to.</param>
		/// <returns>object itself.</returns>
		public override PdfObject MakeIndirect(PdfDocument document, PdfIndirectReference
			 reference)
		{
			return (iTextSharp.Kernel.Pdf.PdfNumber)base.MakeIndirect(document, reference);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document)
		{
			return (iTextSharp.Kernel.Pdf.PdfNumber)base.CopyTo(document, true);
		}

		/// <summary>Copies object to a specified document.</summary>
		/// <remarks>
		/// Copies object to a specified document.
		/// Works only for objects that are read from existing document, otherwise an exception is thrown.
		/// </remarks>
		/// <param name="document">document to copy object to.</param>
		/// <param name="allowDuplicating">
		/// indicates if to allow copy objects which already have been copied.
		/// If object is associated with any indirect reference and allowDuplicating is false then already existing reference will be returned instead of copying object.
		/// If allowDuplicating is true then object will be copied and new indirect reference will be assigned.
		/// </param>
		/// <returns>copied object.</returns>
		public override PdfObject CopyTo(PdfDocument document, bool allowDuplicating)
		{
			return (iTextSharp.Kernel.Pdf.PdfNumber)base.CopyTo(document, allowDuplicating);
		}

		public override String ToString()
		{
			if (content != null)
			{
				return iTextSharp.IO.Util.JavaUtil.GetStringForBytes(content);
			}
			else
			{
				if (isDouble)
				{
					return iTextSharp.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(GetValue
						()));
				}
				else
				{
					return iTextSharp.IO.Util.JavaUtil.GetStringForBytes(ByteUtils.GetIsoBytes(IntValue
						()));
				}
			}
		}

		protected internal override PdfObject NewInstance()
		{
			return new iTextSharp.Kernel.Pdf.PdfNumber();
		}

		protected internal virtual bool IsDoubleNumber()
		{
			return isDouble;
		}

		protected internal override void GenerateContent()
		{
			if (isDouble)
			{
				content = ByteUtils.GetIsoBytes(value);
			}
			else
			{
				content = ByteUtils.GetIsoBytes((int)value);
			}
		}

		protected internal virtual void GenerateValue()
		{
			try
			{
				value = System.Double.Parse(iTextSharp.IO.Util.JavaUtil.GetStringForBytes(content
					));
			}
			catch (FormatException)
			{
				value = Double.NaN;
			}
			isDouble = true;
		}

		protected internal override void CopyContent(PdfObject from, PdfDocument document
			)
		{
			base.CopyContent(from, document);
			iTextSharp.Kernel.Pdf.PdfNumber number = (iTextSharp.Kernel.Pdf.PdfNumber)from;
			value = number.value;
			isDouble = number.isDouble;
		}
	}
}
