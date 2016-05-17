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
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Action
{
	public class PdfTargetDictionary : PdfObjectWrapper<PdfDictionary>
	{
		public PdfTargetDictionary(PdfDictionary pdfObject)
			: base(pdfObject)
		{
		}

		public PdfTargetDictionary(PdfName r)
			: this(new PdfDictionary())
		{
			Put(PdfName.R, r);
		}

		public PdfTargetDictionary(PdfName r, PdfString n, PdfObject p, PdfObject a, iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary
			 t)
			: this(new PdfDictionary())
		{
			Put(PdfName.R, r).Put(PdfName.N, n).Put(PdfName.P, p).Put(PdfName.A, a).Put(PdfName
				.T, t.GetPdfObject());
		}

		/// <summary>Sets the name of the file in the EmbeddedFiles name tree.</summary>
		/// <param name="name">the name of the file</param>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetName(String name
			)
		{
			return Put(PdfName.N, new PdfString(name));
		}

		/// <summary>Gets name of the file</summary>
		/// <returns/>
		public virtual PdfString GetName()
		{
			return GetPdfObject().GetAsString(PdfName.N);
		}

		/// <summary>Sets the page number in the current document containing the file attachment annotation.
		/// 	</summary>
		/// <param name="pageNumber"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetPage(int pageNumber
			)
		{
			return Put(PdfName.P, new PdfNumber(pageNumber));
		}

		/// <summary>Sets a named destination in the current document that provides the page number of the file attachment annotation.
		/// 	</summary>
		/// <param name="namedDestination"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetPage(String namedDestination
			)
		{
			return Put(PdfName.P, new PdfString(namedDestination));
		}

		/// <summary>Get the page number or a named destination that provides the page number containing the file attachment annotation
		/// 	</summary>
		/// <returns/>
		public virtual PdfObject GetPage()
		{
			return GetPdfObject().Get(PdfName.P);
		}

		/// <summary>Sets the index of the annotation in Annots array of the page specified by /P entry.
		/// 	</summary>
		/// <param name="annotNumber"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetAnnotation(int
			 annotNumber)
		{
			return Put(PdfName.A, new PdfNumber(annotNumber));
		}

		/// <summary>Sets the text value, which specifies the value of the /NM entry in the annotation dictionary.
		/// 	</summary>
		/// <param name="annotationName"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetAnnotation(String
			 annotationName)
		{
			return Put(PdfName.A, new PdfString(annotationName));
		}

		public virtual PdfObject GetAnnotation()
		{
			return GetPdfObject().Get(PdfName.A);
		}

		/// <summary>Sets a target dictionary specifying additional path information to the target document.
		/// 	</summary>
		/// <param name="target"/>
		/// <returns/>
		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary SetTarget(iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary
			 target)
		{
			return Put(PdfName.T, target.GetPdfObject());
		}

		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary GetTarget()
		{
			return new iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary(GetPdfObject().GetAsDictionary
				(PdfName.T));
		}

		public virtual iTextSharp.Kernel.Pdf.Action.PdfTargetDictionary Put(PdfName key, 
			PdfObject value)
		{
			GetPdfObject().Put(key, value);
			return this;
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return false;
		}
	}
}
