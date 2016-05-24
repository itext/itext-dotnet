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
using System.Collections.Generic;

namespace iTextSharp.Kernel.Pdf
{
	/// <summary>Enum listing all official PDF versions.</summary>
	public class PdfVersion : IComparable<iTextSharp.Kernel.Pdf.PdfVersion>
	{
		private static readonly IList<iTextSharp.Kernel.Pdf.PdfVersion> values = new List
			<iTextSharp.Kernel.Pdf.PdfVersion>();

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_0 = CreatePdfVersion
			(1, 0);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_1 = CreatePdfVersion
			(1, 0);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_2 = CreatePdfVersion
			(1, 2);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_3 = CreatePdfVersion
			(1, 3);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_4 = CreatePdfVersion
			(1, 4);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_5 = CreatePdfVersion
			(1, 5);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_6 = CreatePdfVersion
			(1, 6);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_1_7 = CreatePdfVersion
			(1, 7);

		public static readonly iTextSharp.Kernel.Pdf.PdfVersion PDF_2_0 = CreatePdfVersion
			(2, 0);

		private int major;

		private int minor;

		/// <summary>Creates a PdfVersion enum.</summary>
		/// <param name="major">major version number</param>
		/// <param name="minor">minor version number</param>
		private PdfVersion(int major, int minor)
		{
			this.major = major;
			this.minor = minor;
		}

		public override String ToString()
		{
			return String.Format("PDF-{0}.{1}", major, minor);
		}

		public virtual PdfName ToPdfName()
		{
			return new PdfName(String.Format("{0}.{1}", major, minor));
		}

		/// <summary>
		/// Creates a PdfVersion enum from a String object if the specified version
		/// can be found.
		/// </summary>
		/// <param name="value">version number</param>
		/// <returns>PdfVersion of the specified version</returns>
		public static iTextSharp.Kernel.Pdf.PdfVersion FromString(String value)
		{
			foreach (iTextSharp.Kernel.Pdf.PdfVersion version in values)
			{
				if (version.ToString().Equals(value))
				{
					return version;
				}
			}
			throw new ArgumentException("The provided pdf version was not found.");
		}

		/// <summary>
		/// Creates a PdfVersion enum from a
		/// <see cref="PdfName"/>
		/// object if the specified version
		/// can be found.
		/// </summary>
		/// <param name="name">version number</param>
		/// <returns>PdfVersion of the specified version</returns>
		public static iTextSharp.Kernel.Pdf.PdfVersion FromPdfName(PdfName name)
		{
			foreach (iTextSharp.Kernel.Pdf.PdfVersion version in values)
			{
				if (version.ToPdfName().Equals(name))
				{
					return version;
				}
			}
			throw new ArgumentException("The provided pdf version was not found.");
		}

		public virtual int CompareTo(iTextSharp.Kernel.Pdf.PdfVersion o)
		{
			int majorResult = iTextSharp.IO.Util.JavaUtil.IntegerCompare(major, o.major);
			if (majorResult != 0)
			{
				return majorResult;
			}
			else
			{
				return iTextSharp.IO.Util.JavaUtil.IntegerCompare(minor, o.minor);
			}
		}

		private static iTextSharp.Kernel.Pdf.PdfVersion CreatePdfVersion(int major, int minor
			)
		{
			iTextSharp.Kernel.Pdf.PdfVersion pdfVersion = new iTextSharp.Kernel.Pdf.PdfVersion
				(major, minor);
			values.Add(pdfVersion);
			return pdfVersion;
		}
	}
}
