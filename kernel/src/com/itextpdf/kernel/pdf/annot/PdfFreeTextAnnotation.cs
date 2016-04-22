/*
$Id: 2965033da6d68ac0831fa412aa490c1986f0ea63 $

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
using com.itextpdf.kernel.geom;
using com.itextpdf.kernel.pdf;

namespace com.itextpdf.kernel.pdf.annot
{
	public class PdfFreeTextAnnotation : PdfMarkupAnnotation
	{
		private const long serialVersionUID = -7835504102518915220L;

		/// <summary>Text justification options.</summary>
		public const int LEFT_JUSTIFIED = 0;

		public const int CENTERED = 1;

		public const int RIGHT_JUSTIFIED = 2;

		public PdfFreeTextAnnotation(Rectangle rect, String appearanceString)
			: base(rect)
		{
			SetDefaultAppearance(new PdfString(appearanceString));
		}

		public PdfFreeTextAnnotation(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		public override PdfName GetSubtype()
		{
			return PdfName.FreeText;
		}

		public virtual PdfString GetDefaultStyleString()
		{
			return GetPdfObject().GetAsString(PdfName.DS);
		}

		public virtual com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation SetDefaultStyleString
			(PdfString defaultStyleString)
		{
			return (com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation)Put(PdfName.DS, defaultStyleString
				);
		}

		public virtual PdfArray GetCalloutLine()
		{
			return GetPdfObject().GetAsArray(PdfName.CL);
		}

		public virtual com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation SetCalloutLine
			(float[] calloutLine)
		{
			return SetCalloutLine(new PdfArray(calloutLine));
		}

		public virtual com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation SetCalloutLine
			(PdfArray calloutLine)
		{
			return (com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation)Put(PdfName.CL, calloutLine
				);
		}

		public virtual PdfName GetLineEndingStyle()
		{
			return GetPdfObject().GetAsName(PdfName.LE);
		}

		public virtual com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation SetLineEndingStyle
			(PdfName lineEndingStyle)
		{
			return (com.itextpdf.kernel.pdf.annot.PdfFreeTextAnnotation)Put(PdfName.LE, lineEndingStyle
				);
		}
	}
}
