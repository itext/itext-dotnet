/*
$Id: 4d9802cc5bee0a9a5d115ad8ff5d75836f6e5866 $

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
using System.Collections.Generic;
using com.itextpdf.kernel.pdf;

namespace com.itextpdf.kernel.utils
{
	public class PdfMerger
	{
		private PdfDocument pdfDocument;

		private IList<PdfMerger.AddedPages> pagesToCopy = new List<PdfMerger.AddedPages>(
			);

		/// <summary>This class is used to merge a number of existing documents into one;</summary>
		/// <param name="pdfDocument">- the document into which source documents will be merged.
		/// 	</param>
		public PdfMerger(PdfDocument pdfDocument)
		{
			this.pdfDocument = pdfDocument;
		}

		/// <summary>This method adds pages from the source document to the List of pages which will be merged.
		/// 	</summary>
		/// <param name="from">- document, from which pages will be copied.</param>
		/// <param name="fromPage">- start page in the range of pages to be copied.</param>
		/// <param name="toPage">- end page in the range to be copied.</param>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual void AddPages(PdfDocument from, int fromPage, int toPage)
		{
			for (int pageNum = fromPage; pageNum <= toPage; pageNum++)
			{
				EnqueuePageToCopy(from, pageNum);
			}
		}

		/// <summary>This method adds pages from the source document to the List of pages which will be merged.
		/// 	</summary>
		/// <param name="from">- document, from which pages will be copied.</param>
		/// <param name="pages">- List of numbers of pages which will be copied.</param>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual void AddPages(PdfDocument from, IList<int> pages)
		{
			foreach (int pageNum in pages)
			{
				EnqueuePageToCopy(from, pageNum);
			}
		}

		/// <summary>This method gets all pages from the List of pages to be copied and merges them into one document.
		/// 	</summary>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		public virtual void Merge()
		{
			foreach (PdfMerger.AddedPages addedPages in pagesToCopy)
			{
				addedPages.from.CopyPagesTo(addedPages.pagesToCopy, pdfDocument);
			}
		}

		/// <summary>This method adds to the List of pages to be copied with given page.</summary>
		/// <remarks>
		/// This method adds to the List of pages to be copied with given page.
		/// Pages are stored along with their documents.
		/// If last added page belongs to the same document as the new one, new page is added to the previous
		/// <c>AddedPages</c>
		/// instance.
		/// </remarks>
		/// <param name="from">- document, from which pages will be copied.</param>
		/// <param name="pageNum">- number of page to be copied.</param>
		/// <exception cref="com.itextpdf.kernel.PdfException"/>
		private void EnqueuePageToCopy(PdfDocument from, int pageNum)
		{
			if (!pagesToCopy.IsEmpty())
			{
				PdfMerger.AddedPages lastAddedPagesEntry = pagesToCopy[pagesToCopy.Count - 1];
				if (lastAddedPagesEntry.from == from)
				{
					lastAddedPagesEntry.pagesToCopy.Add(pageNum);
				}
				else
				{
					pagesToCopy.Add(new PdfMerger.AddedPages(from, pageNum));
				}
			}
			else
			{
				pagesToCopy.Add(new PdfMerger.AddedPages(from, pageNum));
			}
		}

		internal class AddedPages
		{
			public AddedPages(PdfDocument from, int pageNum)
			{
				this.from = from;
				this.pagesToCopy = new List<int>();
				this.pagesToCopy.Add(pageNum);
			}

			internal PdfDocument from;

			internal IList<int> pagesToCopy;
		}
	}
}
