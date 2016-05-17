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
using iTextSharp.IO;
using iTextSharp.IO.Log;
using iTextSharp.Kernel;

namespace iTextSharp.Kernel.Pdf
{
	/// <summary>
	/// Algorithm for construction
	/// <seealso>PdfPages</seealso>
	/// tree
	/// </summary>
	internal class PdfPagesTree
	{
		private const long serialVersionUID = 4189501363348296036L;

		private readonly int leafSize = 10;

		private IList<PdfDictionary> pageRefs;

		private IList<PdfPages> parents;

		private IList<PdfPage> pages;

		private PdfDocument document;

		private bool generated = false;

		private PdfPages root;

		/// <summary>Create PdfPages tree.</summary>
		/// <param name="pdfCatalog">
		/// 
		/// <seealso>PdfCatalog</seealso>
		/// </param>
		public PdfPagesTree(PdfCatalog pdfCatalog)
		{
			this.document = pdfCatalog.GetDocument();
			this.pageRefs = new List<PdfDictionary>();
			this.parents = new List<PdfPages>();
			this.pages = new List<PdfPage>();
			if (pdfCatalog.GetPdfObject().ContainsKey(PdfName.Pages))
			{
				PdfDictionary pages = pdfCatalog.GetPdfObject().GetAsDictionary(PdfName.Pages);
				if (pages == null)
				{
					throw new PdfException(PdfException.InvalidPageStructurePagesPagesMustBePdfDictionary
						);
				}
				this.root = new PdfPages(0, int.MaxValue, pages, null);
				parents.Add(this.root);
				for (int i = 0; i < this.root.GetCount(); i++)
				{
					this.pageRefs.Add(null);
					this.pages.Add(null);
				}
			}
			else
			{
				this.root = null;
				this.parents.Add(new PdfPages(0, this.document));
			}
		}

		//in read mode we will create PdfPages from 0 to Count
		// and reserve null indexes for pageRefs and pages.
		/// <summary>
		/// Returns the
		/// <seealso>PdfPage</seealso>
		/// at the specified position in this list.
		/// </summary>
		/// <param name="pageNum">one-based index of the element to return</param>
		/// <returns>
		/// the
		/// <seealso>PdfPage</seealso>
		/// at the specified position in this list
		/// </returns>
		public virtual PdfPage GetPage(int pageNum)
		{
			--pageNum;
			PdfPage pdfPage = pages[pageNum];
			if (pdfPage == null)
			{
				LoadPage(pageNum);
				pdfPage = new PdfPage(pageRefs[pageNum]);
				int parentIndex = FindPageParent(pageNum);
				PdfPages parentPages = parents[parentIndex];
				pdfPage.parentPages = parentPages;
				pages[pageNum] = pdfPage;
			}
			return pdfPage;
		}

		/// <summary>
		/// Returns the
		/// <seealso>PdfPage</seealso>
		/// by page's PdfDictionary.
		/// </summary>
		/// <param name="pageDictionary">page's PdfDictionary</param>
		/// <returns>
		/// the
		/// <c>PdfPage</c>
		/// object, that wraps
		/// <paramref name="pageDictionary"/>
		/// .
		/// </returns>
		public virtual PdfPage GetPage(PdfDictionary pageDictionary)
		{
			int pageNum = GetPageNumber(pageDictionary);
			if (pageNum > 0)
			{
				return GetPage(pageNum);
			}
			return null;
		}

		/// <summary>Gets total number of @see PdfPages.</summary>
		/// <returns>total number of pages</returns>
		public virtual int GetNumberOfPages()
		{
			return pageRefs.Count;
		}

		/// <summary>
		/// Returns the index of the first occurrence of the specified page
		/// in this tree, or 0 if this tree does not contain the page.
		/// </summary>
		public virtual int GetPageNumber(PdfPage page)
		{
			return pages.IndexOf(page) + 1;
		}

		/// <summary>
		/// Returns the index of the first occurrence of the page in this tree
		/// specified by it's PdfDictionary, or 0 if this tree does not contain the page.
		/// </summary>
		public virtual int GetPageNumber(PdfDictionary pageDictionary)
		{
			int pageNum = pageRefs.IndexOf(pageDictionary);
			if (pageNum >= 0)
			{
				return pageNum + 1;
			}
			for (int i = 0; i < pageRefs.Count; i++)
			{
				if (pageRefs[i] == null)
				{
					LoadPage(i);
				}
				if (pageRefs[i].Equals(pageDictionary))
				{
					return i + 1;
				}
			}
			return 0;
		}

		/// <summary>
		/// Appends the specified
		/// <seealso>PdfPage</seealso>
		/// to the end of this tree.
		/// </summary>
		/// <param name="pdfPage">
		/// 
		/// <seealso>PdfPage</seealso>
		/// </param>
		public virtual void AddPage(PdfPage pdfPage)
		{
			PdfPages pdfPages;
			if (root != null)
			{
				// in this case we save tree structure
				if (pageRefs.IsEmpty())
				{
					pdfPages = root;
				}
				else
				{
					LoadPage(pageRefs.Count - 1);
					pdfPages = parents[parents.Count - 1];
				}
			}
			else
			{
				pdfPages = parents[parents.Count - 1];
				if (pdfPages.GetCount() % leafSize == 0 && !pageRefs.IsEmpty())
				{
					pdfPages = new PdfPages(pdfPages.GetFrom() + pdfPages.GetCount(), document);
					parents.Add(pdfPages);
				}
			}
			pdfPage.MakeIndirect(document);
			pdfPages.AddPage(pdfPage.GetPdfObject());
			pdfPage.parentPages = pdfPages;
			pageRefs.Add(pdfPage.GetPdfObject());
			pages.Add(pdfPage);
		}

		/// <summary>
		/// Insert
		/// <seealso>PdfPage</seealso>
		/// into specific one-based position.
		/// </summary>
		/// <param name="index">one-base index of the page</param>
		/// <param name="pdfPage">
		/// 
		/// <seealso>PdfPage</seealso>
		/// </param>
		public virtual void AddPage(int index, PdfPage pdfPage)
		{
			--index;
			if (index > pageRefs.Count)
			{
				throw new IndexOutOfRangeException("index");
			}
			if (index == pageRefs.Count)
			{
				AddPage(pdfPage);
				return;
			}
			LoadPage(index);
			pdfPage.MakeIndirect(document);
			int parentIndex = FindPageParent(index);
			PdfPages parentPages = parents[parentIndex];
			parentPages.AddPage(index, pdfPage);
			pdfPage.parentPages = parentPages;
			CorrectPdfPagesFromProperty(parentIndex + 1, +1);
			pageRefs.Insert(index, pdfPage.GetPdfObject());
			pages.Insert(index, pdfPage);
		}

		/// <summary>Removes the page at the specified position in this tree.</summary>
		/// <remarks>
		/// Removes the page at the specified position in this tree.
		/// Shifts any subsequent elements to the left (subtracts one from their
		/// indices).
		/// </remarks>
		/// <param name="pageNum">the one-based index of the PdfPage to be removed</param>
		/// <returns>the page that was removed from the list</returns>
		public virtual PdfPage RemovePage(int pageNum)
		{
			PdfPage pdfPage = GetPage(pageNum);
			if (pdfPage.IsFlushed())
			{
				ILogger logger = LoggerFactory.GetLogger(typeof(PdfPage));
				logger.Warn(LogMessageConstant.REMOVING_PAGE_HAS_ALREADY_BEEN_FLUSHED);
			}
			if (InternalRemovePage(--pageNum))
			{
				return pdfPage;
			}
			else
			{
				return null;
			}
		}

		/// <summary>Generate PdfPages tree.</summary>
		/// <returns>
		/// root
		/// <seealso>PdfPages</seealso>
		/// </returns>
		/// <exception cref="iTextSharp.Kernel.PdfException">in case empty document</exception>
		protected internal virtual PdfObject GenerateTree()
		{
			if (pageRefs.IsEmpty())
			{
				throw new PdfException(PdfException.DocumentHasNoPages);
			}
			if (generated)
			{
				throw new PdfException(PdfException.PdfPagesTreeCouldBeGeneratedOnlyOnce);
			}
			if (root == null)
			{
				while (parents.Count != 1)
				{
					IList<PdfPages> nextParents = new List<PdfPages>();
					//dynamicLeafSize helps to avoid PdfPages leaf with only one page
					int dynamicLeafSize = leafSize;
					PdfPages current = null;
					for (int i = 0; i < parents.Count; i++)
					{
						PdfPages pages = parents[i];
						int pageCount = pages.GetCount();
						if (i % dynamicLeafSize == 0)
						{
							if (pageCount <= 1)
							{
								dynamicLeafSize++;
							}
							else
							{
								current = new PdfPages(-1, document);
								nextParents.Add(current);
								dynamicLeafSize = leafSize;
							}
						}
						System.Diagnostics.Debug.Assert(current != null);
						current.AddPages(pages);
					}
					parents = nextParents;
				}
				root = parents[0];
			}
			generated = true;
			return root.GetPdfObject();
		}

		protected internal virtual void ClearPageRefs()
		{
			pageRefs = null;
			pages = null;
		}

		protected internal virtual IList<PdfPages> GetParents()
		{
			return parents;
		}

		protected internal virtual PdfPages GetRoot()
		{
			return root;
		}

		protected internal virtual PdfPages FindPageParent(PdfPage pdfPage)
		{
			int pageNum = GetPageNumber(pdfPage) - 1;
			int parentIndex = FindPageParent(pageNum);
			return parents[parentIndex];
		}

		private void LoadPage(int pageNum)
		{
			PdfDictionary targetPage = pageRefs[pageNum];
			if (targetPage != null)
			{
				return;
			}
			//if we go here, we have to split PdfPages that contains pageNum
			int parentIndex = FindPageParent(pageNum);
			PdfPages parent = parents[parentIndex];
			PdfArray kids = parent.GetKids();
			if (kids == null)
			{
				throw new PdfException(PdfException.InvalidPageStructure1).SetMessageParams(pageNum
					 + 1);
			}
			int kidsCount = parent.GetCount();
			// we should handle separated pages, it means every PdfArray kids must contain either PdfPage or PdfPages,
			// mix of PdfPage and PdfPages not allowed.
			bool findPdfPages = false;
			// NOTE optimization? when we already found needed index
			for (int i = 0; i < kids.Size(); i++)
			{
				PdfDictionary page = kids.GetAsDictionary(i);
				if (page == null)
				{
					// null values not allowed in pages tree.
					throw new PdfException(PdfException.InvalidPageStructure1).SetMessageParams(pageNum
						 + 1);
				}
				PdfObject pageKids = page.Get(PdfName.Kids);
				if (pageKids != null)
				{
					if (pageKids.GetType() == PdfObject.ARRAY)
					{
						findPdfPages = true;
					}
					else
					{
						// kids must be of type array
						throw new PdfException(PdfException.InvalidPageStructure1).SetMessageParams(pageNum
							 + 1);
					}
				}
			}
			if (findPdfPages)
			{
				// handle mix of PdfPage and PdfPages.
				// handle count property!
				IList<PdfPages> newParents = new List<PdfPages>(kids.Size());
				PdfPages lastPdfPages = null;
				for (int i_1 = 0; i_1 < kids.Size() && kidsCount > 0; i_1++)
				{
					PdfDictionary pdfPagesObject = kids.GetAsDictionary(i_1);
					if (pdfPagesObject.GetAsArray(PdfName.Kids) == null)
					{
						// pdfPagesObject is PdfPage
						if (lastPdfPages == null)
						{
							// possible if only first kid is PdfPage
							lastPdfPages = new PdfPages(parent.GetFrom(), document, parent);
							kids.Set(i_1, lastPdfPages.GetPdfObject());
							newParents.Add(lastPdfPages);
						}
						lastPdfPages.AddPage(pdfPagesObject);
						kids.Remove(i_1);
						i_1--;
						kidsCount--;
					}
					else
					{
						// pdfPagesObject is PdfPages
						int from = lastPdfPages == null ? parent.GetFrom() : lastPdfPages.GetFrom() + lastPdfPages
							.GetCount();
						lastPdfPages = new PdfPages(from, kidsCount, pdfPagesObject, parent);
						newParents.Add(lastPdfPages);
						kidsCount -= lastPdfPages.GetCount();
					}
				}
				parents.RemoveAt(parentIndex);
				for (int i_2 = newParents.Count - 1; i_2 >= 0; i_2--)
				{
					parents.Insert(parentIndex, newParents[i_2]);
				}
				// recursive call, to load needed pageRef.
				// NOTE optimization? add to loadPage startParentIndex.
				LoadPage(pageNum);
			}
			else
			{
				int from = parent.GetFrom();
				// Possible exception in case kids.getSize() < parent.getCount().
				// In any case parent.getCount() has higher priority.
				// NOTE optimization? when we already found needed index
				for (int i_1 = 0; i_1 < parent.GetCount(); i_1++)
				{
					pageRefs[from + i_1] = kids.GetAsDictionary(i_1);
				}
			}
		}

		// zero-based index
		private bool InternalRemovePage(int pageNum)
		{
			int parentIndex = FindPageParent(pageNum);
			PdfPages pdfPages = parents[parentIndex];
			if (pdfPages.RemovePage(pageNum))
			{
				if (pdfPages.GetCount() == 0)
				{
					parents.RemoveAt(parentIndex);
					pdfPages.RemoveFromParent();
					--parentIndex;
				}
				if (parents.Count == 0)
				{
					root = null;
					parents.Add(new PdfPages(0, document));
				}
				else
				{
					CorrectPdfPagesFromProperty(parentIndex + 1, -1);
				}
				pageRefs.RemoveAt(pageNum);
				pages.RemoveAt(pageNum);
				return true;
			}
			else
			{
				return false;
			}
		}

		// zero-based index
		private int FindPageParent(int pageNum)
		{
			int low = 0;
			int high = parents.Count - 1;
			while (low != high)
			{
				int middle = (low + high + 1) / 2;
				if (parents[middle].CompareTo(pageNum) > 0)
				{
					high = middle - 1;
				}
				else
				{
					low = middle;
				}
			}
			return low;
		}

		private void CorrectPdfPagesFromProperty(int index, int correction)
		{
			for (int i = index; i < parents.Count; i++)
			{
				if (parents[i] != null)
				{
					parents[i].CorrectFrom(correction);
				}
			}
		}
	}
}
