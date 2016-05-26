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
using System.Collections.Generic;
using iTextSharp.Kernel;
using iTextSharp.Kernel.Pdf;

namespace iTextSharp.Kernel.Pdf.Tagging
{
	/// <summary>
	/// Internal helper class which is used to effectively build parent tree and also find marked content references:
	/// for specified page, by MCID or by struct parent index.
	/// </summary>
	internal class ParentTreeHandler
	{
		private PdfStructTreeRoot structTreeRoot;

		/// <summary>Represents parentTree in structTreeRoot.</summary>
		/// <remarks>Represents parentTree in structTreeRoot. It contains only those entries that belong to the already flushed pages.
		/// 	</remarks>
		private PdfNumTree parentTree;

		/// <summary>Contains marked content references for every page.</summary>
		/// <remarks>
		/// Contains marked content references for every page.
		/// If new mcrs are added to the tag structure, these new mcrs are also added to this map. So for each adding or
		/// removing mcr, register/unregister calls must be made (this is done automatically if addKid or removeKid methods
		/// of PdfStructElem are used).
		/// Keys in this map are page references, values - a map which contains all mcrs that belong to the given page.
		/// This inner map of mcrs is of following structure:
		/// * for McrDictionary and McrNumber values the keys are their MCIDs;
		/// * for ObjRef values the keys are struct parent indexes, but with one trick. Struct parent indexes and MCIDs have the
		/// same value domains: the increasing numbers starting from zero. So, in order to store them in one map, for
		/// struct parent indexes simple transformation is applied via
		/// <c>#structParentIndexIntoKey</c>
		/// and
		/// <c>#keyIntoStructParentIndex</c>
		/// . With this we simply store struct parent indexes as negative numbers.
		/// </remarks>
		private IDictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr>> pageToPageMcrs;

		/// <summary>Init ParentTreeHandler.</summary>
		/// <remarks>Init ParentTreeHandler. On init the parent tree is read and stored in this instance.
		/// 	</remarks>
		internal ParentTreeHandler(PdfStructTreeRoot structTreeRoot)
		{
			this.structTreeRoot = structTreeRoot;
			parentTree = new PdfNumTree(structTreeRoot.GetDocument().GetCatalog(), PdfName.ParentTree
				);
			RegisterAllMcrs();
		}

		/// <summary>Gets a list of marked content references on page.</summary>
		public virtual IDictionary<int, PdfMcr> GetPageMarkedContentReferences(PdfPage page
			)
		{
			return pageToPageMcrs[page.GetPdfObject().GetIndirectReference()];
		}

		public virtual PdfMcr FindMcrByMcid(PdfDictionary pageDict, int mcid)
		{
			IDictionary<int, PdfMcr> pageMcrs = pageToPageMcrs[pageDict.GetIndirectReference(
				)];
			return pageMcrs != null ? pageMcrs[mcid] : null;
		}

		public virtual PdfObjRef FindObjRefByStructParentIndex(PdfDictionary pageDict, int
			 structParentIndex)
		{
			IDictionary<int, PdfMcr> pageMcrs = pageToPageMcrs[pageDict.GetIndirectReference(
				)];
			return pageMcrs != null ? (PdfObjRef)pageMcrs[StructParentIndexIntoKey(structParentIndex
				)] : null;
		}

		public virtual int GetNextMcidForPage(PdfPage page)
		{
			SortedDictionary<int, PdfMcr> pageMcrs = pageToPageMcrs[page.GetPdfObject().GetIndirectReference
				()];
			if (pageMcrs == null || pageMcrs.Count == 0)
			{
				return 0;
			}
			else
			{
				int lastKey = (int)System.Linq.Enumerable.Last(pageMcrs).Key;
				if (lastKey < 0)
				{
					return 0;
				}
				return lastKey + 1;
			}
		}

		/// <summary>Creates and flushes parent tree entry for the page.</summary>
		/// <remarks>
		/// Creates and flushes parent tree entry for the page.
		/// Effectively this means that new content mustn't be added to the page.
		/// </remarks>
		/// <param name="page">
		/// 
		/// <see cref="iTextSharp.Kernel.Pdf.PdfPage"/>
		/// for which to create parent tree entry. Typically this page is flushed after this call.
		/// </param>
		public virtual void CreateParentTreeEntryForPage(PdfPage page)
		{
			IDictionary<int, PdfMcr> mcrs = GetPageMarkedContentReferences(page);
			if (mcrs == null)
			{
				return;
			}
			pageToPageMcrs.JRemove(page.GetPdfObject().GetIndirectReference());
			UpdateStructParentTreeEntries(page.GetStructParentIndex(), mcrs);
		}

		public virtual PdfDictionary BuildParentTree()
		{
			return ((PdfDictionary)parentTree.BuildTree().MakeIndirect(structTreeRoot.GetDocument
				()));
		}

		public virtual void RegisterMcr(PdfMcr mcr)
		{
			SortedDictionary<int, PdfMcr> pageMcrs = pageToPageMcrs[mcr.GetPageObject().GetIndirectReference
				()];
			if (pageMcrs == null)
			{
				pageMcrs = new SortedDictionary<int, PdfMcr>();
				pageToPageMcrs[mcr.GetPageObject().GetIndirectReference()] = pageMcrs;
			}
			if (mcr is PdfObjRef)
			{
				PdfDictionary obj = ((PdfDictionary)mcr.GetPdfObject()).GetAsDictionary(PdfName.Obj
					);
				if (obj == null || obj.IsFlushed())
				{
					throw new PdfException(PdfException.WhenAddingObjectReferenceToTheTagTreeItMustBeConnectedToNotFlushedObject
						);
				}
				PdfNumber n = obj.GetAsNumber(PdfName.StructParent);
				if (n != null)
				{
					pageMcrs[StructParentIndexIntoKey(n.IntValue())] = mcr;
				}
				else
				{
					throw new PdfException(PdfException.StructParentIndexNotFoundInTaggedObject);
				}
			}
			else
			{
				pageMcrs[mcr.GetMcid()] = mcr;
			}
		}

		public virtual void UnregisterMcr(PdfMcr mcrToUnregister)
		{
			PdfDictionary pageDict = mcrToUnregister.GetPageObject();
			if (pageDict.IsFlushed())
			{
				throw new PdfException(PdfException.CannotRemoveMarkedContentReferenceBecauseItsPageWasAlreadyFlushed
					);
			}
			IDictionary<int, PdfMcr> pageMcrs = pageToPageMcrs[pageDict.GetIndirectReference(
				)];
			if (pageMcrs != null)
			{
				if (mcrToUnregister is PdfObjRef)
				{
					PdfDictionary obj = ((PdfDictionary)mcrToUnregister.GetPdfObject()).GetAsDictionary
						(PdfName.Obj);
					if (obj != null && !obj.IsFlushed())
					{
						PdfNumber n = obj.GetAsNumber(PdfName.StructParent);
						if (n != null)
						{
							pageMcrs.JRemove(StructParentIndexIntoKey(n.IntValue()));
							return;
						}
					}
					foreach (KeyValuePair<int, PdfMcr> entry in pageMcrs)
					{
						if (entry.Value.GetPdfObject() == mcrToUnregister.GetPdfObject())
						{
							pageMcrs.JRemove(entry.Key);
							break;
						}
					}
				}
				else
				{
					pageMcrs.JRemove(mcrToUnregister.GetMcid());
				}
			}
		}

		private static int StructParentIndexIntoKey(int structParentIndex)
		{
			return -structParentIndex - 1;
		}

		private static int KeyIntoStructParentIndex(int key)
		{
			return -key - 1;
		}

		private void RegisterAllMcrs()
		{
			pageToPageMcrs = new Dictionary<PdfIndirectReference, SortedDictionary<int, PdfMcr
				>>();
			// we create new number tree and not using parentTree, because we want parentTree to be empty
			IDictionary<int?, PdfObject> parentTreeEntries = new PdfNumTree(structTreeRoot.GetDocument
				().GetCatalog(), PdfName.ParentTree).GetNumbers();
			ICollection<PdfStructElem> mcrParents = new HashSet<PdfStructElem>();
			int maxStructParentIndex = -1;
			foreach (KeyValuePair<int?, PdfObject> entry in parentTreeEntries)
			{
				if (entry.Key > maxStructParentIndex)
				{
					maxStructParentIndex = (int)entry.Key;
				}
				PdfObject entryValue = entry.Value;
				if (entryValue.IsDictionary())
				{
					mcrParents.Add(new PdfStructElem((PdfDictionary)entryValue));
				}
				else
				{
					if (entryValue.IsArray())
					{
						PdfArray parentsArray = (PdfArray)entryValue;
						for (int i = 0; i < parentsArray.Size(); ++i)
						{
							PdfDictionary parent = parentsArray.GetAsDictionary(i);
							if (parent != null)
							{
								mcrParents.Add(new PdfStructElem(parent));
							}
						}
					}
				}
			}
			structTreeRoot.GetPdfObject().Put(PdfName.ParentTreeNextKey, new PdfNumber(maxStructParentIndex
				 + 1));
			foreach (PdfStructElem mcrParent in mcrParents)
			{
				foreach (IPdfStructElem kid in mcrParent.GetKids())
				{
					if (kid is PdfMcr)
					{
						RegisterMcr((PdfMcr)kid);
					}
				}
			}
		}

		private void UpdateStructParentTreeEntries(int? pageStructParentIndex, IDictionary
			<int, PdfMcr> mcrs)
		{
			// element indexes in parentsOfPageMcrs shall be the same as mcid of one of their kids.
			// See "Finding Structure Elements from Content Items" in pdf spec.
			PdfArray parentsOfPageMcrs = new PdfArray();
			int currentMcid = 0;
			foreach (KeyValuePair<int, PdfMcr> entry in mcrs)
			{
				PdfMcr mcr = entry.Value;
				if (mcr is PdfObjRef)
				{
					int structParent = KeyIntoStructParentIndex((int)entry.Key);
					parentTree.AddEntry(structParent, ((PdfStructElem)mcr.GetParent()).GetPdfObject()
						);
				}
				else
				{
					// if for some reason some mcr where not registered or don't exist, we ensure that the rest
					// of the parent objects were placed at correct index
					while (currentMcid++ < mcr.GetMcid())
					{
						parentsOfPageMcrs.Add(PdfNull.PDF_NULL);
					}
					parentsOfPageMcrs.Add(((PdfStructElem)mcr.GetParent()).GetPdfObject());
				}
			}
			if (parentsOfPageMcrs.Size() > 0)
			{
				parentsOfPageMcrs.MakeIndirect(structTreeRoot.GetDocument());
				parentTree.AddEntry(pageStructParentIndex, parentsOfPageMcrs);
				parentsOfPageMcrs.Flush();
			}
		}
	}
}
