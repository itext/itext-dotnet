/*

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
using System.IO;
using iTextSharp.IO.Font;
using iTextSharp.IO.Util;
using iTextSharp.Kernel.Pdf;
using iTextSharp.Kernel.Pdf.Collection;

namespace iTextSharp.Kernel.Pdf.Filespec
{
	public class PdfFileSpec : PdfObjectWrapper<PdfObject>
	{
		protected internal PdfFileSpec(PdfObject pdfObject)
			: base(pdfObject)
		{
		}

		public static iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec CreateExternalFileSpec(PdfDocument
			 doc, String filePath, bool isUnicodeFileName)
		{
			PdfDictionary dict = new PdfDictionary();
			dict.Put(PdfName.Type, PdfName.Filespec);
			dict.Put(PdfName.F, new PdfString(filePath));
			dict.Put(PdfName.UF, new PdfString(filePath, isUnicodeFileName ? PdfEncodings.UNICODE_BIG
				 : PdfEncodings.PDF_DOC_ENCODING));
			return (iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec)new iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec
				(dict).MakeIndirect(doc);
		}

		public static iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec CreateEmbeddedFileSpec(PdfDocument
			 doc, byte[] fileStore, String description, String fileDisplay, PdfName mimeType
			, PdfDictionary fileParameter, PdfName afRelationshipValue, bool isUnicodeFileName
			)
		{
			PdfStream stream = ((PdfStream)new PdfStream(fileStore).MakeIndirect(doc));
			PdfDictionary @params = new PdfDictionary();
			if (fileParameter != null)
			{
				@params.MergeDifferent(fileParameter);
			}
			if (!@params.ContainsKey(PdfName.ModDate))
			{
				@params.Put(PdfName.ModDate, new PdfDate().GetPdfObject());
			}
			if (fileStore != null)
			{
				@params.Put(PdfName.Size, new PdfNumber(stream.GetBytes().Length));
				stream.Put(PdfName.Params, @params);
			}
			return CreateEmbeddedFileSpec(doc, stream, description, fileDisplay, mimeType, afRelationshipValue
				, isUnicodeFileName);
		}

		/// <exception cref="System.IO.IOException"/>
		public static iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec CreateEmbeddedFileSpec(PdfDocument
			 doc, String filePath, String description, String fileDisplay, PdfName mimeType, 
			PdfName afRelationshipValue, bool isUnicodeFileName)
		{
			PdfStream stream = new PdfStream(doc, iTextSharp.IO.Util.UrlUtil.OpenStream(UrlUtil
				.ToURL(filePath)));
			return CreateEmbeddedFileSpec(doc, stream, description, fileDisplay, mimeType, afRelationshipValue
				, isUnicodeFileName);
		}

		public static iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec CreateEmbeddedFileSpec(PdfDocument
			 doc, Stream @is, String description, String fileDisplay, PdfName mimeType, PdfName
			 afRelationshipValue, bool isUnicodeFileName)
		{
			PdfStream stream = new PdfStream(doc, @is);
			return CreateEmbeddedFileSpec(doc, stream, description, fileDisplay, mimeType, afRelationshipValue
				, isUnicodeFileName);
		}

		private static iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec CreateEmbeddedFileSpec(
			PdfDocument doc, PdfStream stream, String description, String fileDisplay, PdfName
			 mimeType, PdfName afRelationshipValue, bool isUnicodeFileName)
		{
			PdfDictionary dict = new PdfDictionary();
			stream.Put(PdfName.Type, PdfName.EmbeddedFile);
			if (afRelationshipValue != null)
			{
				dict.Put(PdfName.AFRelationship, afRelationshipValue);
			}
			else
			{
				dict.Put(PdfName.AFRelationship, PdfName.Unspecified);
			}
			if (mimeType != null)
			{
				stream.Put(PdfName.Subtype, mimeType);
			}
			else
			{
				stream.Put(PdfName.Subtype, PdfName.ApplicationOctetStream);
			}
			if (description != null)
			{
				dict.Put(PdfName.Desc, new PdfString(description));
			}
			dict.Put(PdfName.Type, PdfName.Filespec);
			dict.Put(PdfName.F, new PdfString(fileDisplay));
			dict.Put(PdfName.UF, new PdfString(fileDisplay, isUnicodeFileName ? PdfEncodings.
				UNICODE_BIG : PdfEncodings.PDF_DOC_ENCODING));
			PdfDictionary ef = new PdfDictionary();
			ef.Put(PdfName.F, stream);
			ef.Put(PdfName.UF, stream);
			dict.Put(PdfName.EF, ef);
			return (iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec)new iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec
				(dict).MakeIndirect(doc);
		}

		public virtual iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec SetFileIdentifier(PdfArray
			 fileIdentifier)
		{
			return Put(PdfName.ID, fileIdentifier);
		}

		public virtual PdfArray GetFileIdentifier()
		{
			return ((PdfDictionary)GetPdfObject()).GetAsArray(PdfName.ID);
		}

		public virtual iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec SetVolatile(PdfBoolean 
			isVolatile)
		{
			return Put(PdfName.Volatile, isVolatile);
		}

		public virtual PdfBoolean IsVolatile()
		{
			return ((PdfDictionary)GetPdfObject()).GetAsBoolean(PdfName.Volatile);
		}

		public virtual iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec SetCollectionItem(PdfCollectionItem
			 item)
		{
			return Put(PdfName.CI, item.GetPdfObject());
		}

		public virtual iTextSharp.Kernel.Pdf.Filespec.PdfFileSpec Put(PdfName key, PdfObject
			 value)
		{
			((PdfDictionary)GetPdfObject()).Put(key, value);
			return this;
		}

		protected internal override bool IsWrappedObjectMustBeIndirect()
		{
			return true;
		}
	}
}
