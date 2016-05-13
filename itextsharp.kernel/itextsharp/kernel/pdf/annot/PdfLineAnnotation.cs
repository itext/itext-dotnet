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
using iTextSharp.Kernel.Geom;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Annot
{
	public class PdfLineAnnotation : PdfMarkupAnnotation
	{
		private const long serialVersionUID = -6047928061827404283L;

		public PdfLineAnnotation(Rectangle rect, float[] line)
			: base(rect)
		{
			Put(PdfName.L, new PdfArray(line));
		}

		public PdfLineAnnotation(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		public override PdfName GetSubtype()
		{
			return PdfName.Line;
		}

		public virtual PdfArray GetLine()
		{
			return GetPdfObject().GetAsArray(PdfName.L);
		}

		public virtual PdfArray GetLineEndingStyles()
		{
			return GetPdfObject().GetAsArray(PdfName.LE);
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetLineEndingStyles(
			PdfArray lineEndingStyles)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LE, lineEndingStyles
				);
		}

		public virtual float GetLeaderLine()
		{
			PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LE);
			return n == null ? 0 : n.FloatValue();
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLine(float 
			leaderLine)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LE, new PdfNumber
				(leaderLine));
		}

		public virtual float GetLeaderLineExtension()
		{
			PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LLE);
			return n == null ? 0 : n.FloatValue();
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLineExtension
			(float leaderLineExtension)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LLE, new PdfNumber
				(leaderLineExtension));
		}

		public virtual float GetLeaderLineOffset()
		{
			PdfNumber n = GetPdfObject().GetAsNumber(PdfName.LLO);
			return n == null ? 0 : n.FloatValue();
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetLeaderLineOffset(
			float leaderLineOffset)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.LLO, new PdfNumber
				(leaderLineOffset));
		}

		public virtual bool GetContentsAsCaption()
		{
			PdfBoolean b = GetPdfObject().GetAsBoolean(PdfName.Cap);
			return b != null && b.GetValue();
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetContentsAsCaption
			(bool contentsAsCaption)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.Cap, new PdfBoolean
				(contentsAsCaption));
		}

		public virtual PdfName GetCaptionPosition()
		{
			return GetPdfObject().GetAsName(PdfName.CP);
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionPosition(PdfName
			 captionPosition)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.CP, captionPosition
				);
		}

		public virtual PdfDictionary GetMeasure()
		{
			return GetPdfObject().GetAsDictionary(PdfName.Measure);
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetMeasure(PdfDictionary
			 measure)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.Measure, measure
				);
		}

		public virtual PdfArray GetCaptionOffset()
		{
			return GetPdfObject().GetAsArray(PdfName.CO);
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionOffset(PdfArray
			 captionOffset)
		{
			return (iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation)Put(PdfName.CO, captionOffset
				);
		}

		public virtual iTextSharp.Kernel.Pdf.Annot.PdfLineAnnotation SetCaptionOffset(float
			[] captionOffset)
		{
			return SetCaptionOffset(new PdfArray(captionOffset));
		}
	}
}
